# Local AI Workspace

[![.NET](https://img.shields.io/badge/.NET-6%2B-blueviolet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/UI-WPF-5C2D91)](https://learn.microsoft.com/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/license-educational-lightgrey)](#license)
[![Status](https://img.shields.io/badge/status-active-success)](#)

A WPF desktop app for comparing two codebases, generating **deterministic diffs**, and producing **AI-assisted, non-hallucinating explanations**—with built-in static analysis.

---

## ✨ Features

* 🔍 **Folder Comparison**

  * Load **Folder A** and **Folder B**
  * Match files by name
  * Multi-file comparison

* ⚙️ **Deterministic Diff (DiffPlex)**

  * Added / Removed / Modified lines
  * No AI involved in parsing

* 🧠 **AI Explanation (Strict Mode)**

  * AI **only explains**, never parses
  * Line-by-line, literal behavior
  * Guardrails prevent:

    * Hallucination
    * System-level assumptions
    * Grouping/summarizing

* 📊 **Static Analysis**

  * ⚠ Duplicate calls
  * ⚠ Unused return values
  * ⚠ Duplicate variable declarations

* 🧩 **Method Grouping**

  * Groups changes by method/object
  * Surfaces logical clusters

---

## 🏗️ Architecture

```text
DiffPlex (truth)
    ↓
collectedChanges (structured data)
    ↓
rawBuilder (deterministic formatting)
    ↓
AI (strict explanation only)
    ↓
Analysis rules (validation)
    ↓
UI output
```

> **Key principle:** AI never parses raw code—only explains pre-structured input.

---

## 🚀 Getting Started

### Prerequisites

* .NET 6+ (or compatible)
* Windows (WPF)
* DiffPlex NuGet package
* AI service (OpenAI or local equivalent)

### Install

```bash
git clone https://github.com/your-username/local-ai-workspace.git
cd local-ai-workspace
```

Open the solution in Visual Studio and restore NuGet packages.

---

## ▶️ Usage

1. Launch the app
2. Click **Select Folder A**
3. Click **Select Folder B**
4. Choose a file
5. Click:

   * **Explain** → project overview
   * **Compare** → diff + explanation + analysis

---

## 📌 Example Output

```text
- Change:
  Evidence: decimal result = this.ContextA.Service.CalculateValue(input)
  Explanation: Calls CalculateValue(input) and assigns its return value to result

- Change:
  Evidence: this.ContextA.Service.Execute(input)
  Explanation: Calls Execute(input) without assigning the return value

--- Analysis ---
⚠ Unused return: this.ContextA.Service.Execute(input)
```

---

## 📁 Project Structure

```text
LocalAIWorkspace/
│
├── Models/
│   └── CodeFile.cs
│
├── Services/
│   ├── FileService.cs
│   └── AiService.cs
│
├── ViewModels/
│   └── MainViewModel.cs
│
├── MainWindow.xaml
├── MainWindow.xaml.cs
└── README.md
```

---

## 🧠 Design Principles

* **Deterministic first** (DiffPlex is the source of truth)
* **AI is constrained** (explain-only, no inference)
* **No hallucination pipeline**
* **Separation of concerns**
* **Readable over clever**

---

## 🛠️ Tech Stack

* **WPF** (.NET)
* **DiffPlex**
* **C# / MVVM**
* **AI Service** (OpenAI or local)

---

## 🔮 Roadmap

* 🎨 Color-coded diff (green/red/yellow)
* 📍 Click-to-navigate (file/line)
* 📊 Severity scoring
* 🧭 Side-by-side diff viewer
* 🔍 Filters (risk, change type)

---

## 📸 Screenshots

> Add screenshots here
> Example:
> ![App Screenshot](docs/screenshot.png)

---

## 🤝 Contributing

Contributions are welcome. Open an issue or submit a PR.

---

## 📄 License

Educational / personal use.

---

## 👤 Author

**Chris Mathis**
