# Aralash

Windows-приложение для выбора и одновременного запуска нескольких `.exe`-файлов.

## Запуск

Скачайте `aralash.exe` и запустите его. Нажмите **«Обзор...»**, выберите один или несколько файлов (удерживайте `Ctrl` или `Shift` для выбора нескольких), затем нажмите **«Запустить все»**.

## Сборка из исходного кода

Для сборки нужны Windows и компилятор C# из .NET Framework:

```powershell
csc.exe /nologo /target:winexe /out:aralash.exe /win32icon:hacker.ico /reference:System.Windows.Forms.dll /reference:System.Drawing.dll exe-launcher.cs
```

## Файлы проекта

- `aralash.exe` — готовое приложение.
- `exe-launcher.cs` — исходный код приложения.
- `exe-launcher.ps1` — вариант приложения на PowerShell.
- `hacker.ico` — иконка приложения.
- `run.bat` — запускает `aralash.exe`.
