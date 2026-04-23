using EventAlarmDemo;
using System.Runtime.InteropServices;
using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Clock myClock = new Clock();

        // 订阅嘀嗒事件：显示当前时间
        myClock.Tick += (sender, e) =>
        {
            Console.WriteLine($"[嘀嗒]: 当前时间 {DateTime.Now:HH:mm:ss}");
        };

        // 订阅响铃事件：打印特别提示
        myClock.Alarm += (sender, e) =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n****************************");
            Console.WriteLine($"!!! 闹钟响了 !!! {e.Message}");
            Console.WriteLine("****************************\n");
            Console.ResetColor();
        };

        // 设置一个 5 秒后的闹钟作为演示
        DateTime targetTime = DateTime.Now.AddSeconds(5);
        Console.WriteLine($"设定闹钟为: {targetTime:HH:mm:ss}");
        myClock.SetAlarm(targetTime);
        myClock.Run();
    }
}



namespace EventAlarmDemo
{
    // 自定义事件参数，用于传递闹钟响铃时的信息
    public class AlarmEventArgs : EventArgs
    {
        public DateTime AlarmTime { get; }
        public string Message { get; }

        public AlarmEventArgs(DateTime time, string msg)
        {
            AlarmTime = time;
            Message = msg;
        }
    }

    public class Clock
    {
        // 定义事件：使用标准的 EventHandler 委托
        public event EventHandler Tick; // 嘀嗒事件
        public event EventHandler<AlarmEventArgs> Alarm; // 响铃事件

        private DateTime _alarmTime;
        private bool _isAlarmSet = false;

        // 设置闹钟时间
        public void SetAlarm(DateTime time)
        {
            _alarmTime = time;
            _isAlarmSet = true;
        }

        // 启动闹钟运行
        public void Run()
        {
            Console.WriteLine("闹钟已启动...");
            while (true)
            {
                DateTime now = DateTime.Now;

                // 1. 触发嘀嗒事件 (每秒一次)
                OnTick();

                // 2. 检测并触发响铃事件
                if (_isAlarmSet &&
                    now.Hour == _alarmTime.Hour &&
                    now.Minute == _alarmTime.Minute &&
                    now.Second == _alarmTime.Second)
                {
                    OnAlarm(new AlarmEventArgs(now, "起床啦！时间到！"));
                    _isAlarmSet = false; // 响铃后关闭，防止同一秒内重复触发
                }

                Thread.Sleep(1000); // 模拟秒针跳动
            }
        }

        // 触发 Tick 的保护方法
        protected virtual void OnTick()
        {
            // 检查是否有订阅者，如果有则调用
            Tick?.Invoke(this, EventArgs.Empty);
        }

        // 触发 Alarm 的保护方法
        protected virtual void OnAlarm(AlarmEventArgs e)
        {
            Alarm?.Invoke(this, e);
        }
    }
}