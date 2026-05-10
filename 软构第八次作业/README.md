# 背单词程序 (FlashWord)

一款基于 WinForms 和 SQLite 数据库的小型背单词应用。

## 功能介绍

### 核心功能

1. **中文提示，英文作答**
   - 界面中央显示中文词义
   - 用户在输入框中输入对应的英文单词

2. **点击按钮判定**
   - 输入完毕后点击「下一个」按钮判定答案
   - 也可直接按回车键，效果相同

3. **即时反馈**
   - 答案正确：显示绿色文字 "✓ 正确"
   - 答案错误：显示红色文字 "✗ 错误，正确答案: xxx"

4. **进度追踪**
   - 左下角显示当前进度，如 "进度: 1/10"
   - 完成所有单词后显示 "已完成全部单词!"

5. **随机顺序**
   - 每次启动程序，单词顺序随机打乱
   - 避免按固定顺序记忆

6. **自动跳转**
   - 点击「下一个」后，先显示判定结果
   - 等待1秒后自动切换到下一个单词

### 数据存储

- 使用 SQLite 数据库存储单词数据
- 数据库文件：`words.db`（首次运行自动创建）
- 预置 10 个基础单词

## 项目结构

```
FlashWord/
├── FlashWord.csproj          # 项目配置文件，定义依赖包
├── Program.cs                # 程序入口，初始化数据库
├── Word.cs                   # 单词实体类
├── WordContext.cs            # 数据库上下文，管理数据操作
├── Form1.cs                  # 主窗体逻辑代码
├── Form1.Designer.cs         # 主窗体界面设计代码
└── obj/                      # 编译生成的临时文件
```

## 代码文件详解

### Program.cs

**作用**：应用程序入口点

**功能**：
- 创建数据库上下文
- 调用 `EnsureCreated()` 确保数据库和表已创建（首次运行自动创建）
- 初始化 WinForms 应用并启动主窗体

```csharp
using var context = new WordContext();
context.Database.EnsureCreated();
ApplicationConfiguration.Initialize();
Application.Run(new Form1());
```

### Word.cs

**作用**：单词数据模型（实体类）

**功能**：
- 定义 `Word` 类的数据结构
- 包含三个属性：`Id`（主键）、`English`（英文）、`Chinese`（中文）

```csharp
public class Word
{
    public int Id { get; set; }
    public string English { get; set; } = string.Empty;
    public string Chinese { get; set; } = string.Empty;
}
```

### WordContext.cs

**作用**：Entity Framework Core 数据库上下文

**功能**：
- 继承 `DbContext`，提供数据库操作接口
- 定义 `Words` DbSet 集合，用于CRUD操作
- 配置 SQLite 数据库连接，数据库文件为 `words.db`
- 使用 `HasData` 方法预置 10 个单词数据

### Form1.cs

**作用**：主窗体业务逻辑

**功能**：
- `LoadWords()`：从数据库加载所有单词，并随机排序
- `ShowCurrentWord()`：显示当前单词的中文和进度
- `CheckAnswer()`：比较用户输入与正确答案，忽略大小写
- `txtAnswer_KeyDown`：监听回车键，触发下一题
- `btnNext_Click`：点击按钮，先判定答案，等待1秒后切换到下一题

### Form1.Designer.cs

**作用**：主窗体界面设计代码

**功能**：
- 定义 UI 控件：
  - `lblChinese`：显示中文词义（大字体的标签）
  - `txtAnswer`：用户输入英文的文本框
  - `lblResult`：显示判定结果的标签
  - `btnNext`：切换下一个单词的按钮
  - `lblProgress`：显示进度的标签
- 设置控件的位置、大小、字体等属性
- 为文本框绑定键盘事件，为按钮绑定点击事件

## 数据库预置单词

| Id | English | Chinese |
|----|---------|---------|
| 1  | apple   | 苹果    |
| 2  | banana  | 香蕉    |
| 3  | orange  | 橙子    |
| 4  | computer| 电脑    |
| 5  | book    | 书      |
| 6  | phone   | 手机    |
| 7  | table   | 桌子    |
| 8  | chair   | 椅子    |
| 9  | window  | 窗户    |
| 10 | door    | 门      |

## 运行方式

### 在 Visual Studio 中运行

1. 打开 VS，选择 **文件 → 打开 → 项目/解决方案**
2. 选择 `FlashWord/FlashWord.csproj`
3. 按 **F5** 运行

### 直接运行可执行文件

1. 进入 `publish` 目录
2. 双击 `FlashWord.exe`

## publish 目录说明

`publish` 目录是程序发布后生成的文件夹，包含可直接运行的可执行文件和所有依赖项。

```
publish/
├── FlashWord.exe              # 主程序入口，双击运行
├── FlashWord.dll             # 程序主程序集
├── words.db                  # SQLite 数据库文件（首次运行自动创建）
├── runtimes/                 # 各平台运行时依赖
│   └── win-x64/              # Windows x64 平台
│       └── native/           # 原生库（如 SQLite）
└── [其他 DLL 文件]           # .NET 运行时和依赖库
```

**特点**：
- **自包含部署**：包含完整的 .NET 运行时，无需用户在电脑上安装 .NET
- **即点即用**：直接双击 `FlashWord.exe` 即可运行程序
- **数据库文件**：首次运行后会自动生成 `words.db` 文件存储单词数据

**使用步骤**：
1. 将整个 `publish` 文件夹拷贝到目标电脑
2. 进入文件夹，双击 `FlashWord.exe`
3. 首次运行会自动创建数据库

## 技术栈

- **.NET 10** (Windows Desktop)
- **WinForms** - Windows 桌面 UI 框架
- **Entity Framework Core 10** - ORM 框架
- **SQLite** - 轻量级数据库
