# Pasta

A simple WPF tool that pastes pre-configured text and images into the target window with one click.

**Platform:** Windows x64

## How it works

1. Click **Open** to open the data folder (`%LocalAppData%\Pasta\data`)
2. Edit `text.txt` and put images in the `images` folder
3. Click **Paste** — Pasta will simulate Alt+Tab to switch to the previous window, then simulate Ctrl+V to paste the text and images

> **Note:** Firefox only supports pasting the first image. Use a Chromium-based browser (Chrome, Edge, etc.) for full functionality.

## Install

Requires [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0).

Download the latest release, extract, and run `install.bat`. It will:
- Create `%LocalAppData%\Pasta` folder
- Copy binaries to `%LocalAppData%\Pasta`
- Create a desktop shortcut

## Build

```
dotnet build
```

```
dotnet publish Pasta.csproj -c Release -o ./release/bin
```

## Credits

- App icon (`icon.svg`) from [Twemoji](https://github.com/twitter/twemoji) by Twitter, licensed under [CC-BY 4.0](https://creativecommons.org/licenses/by/4.0/)
- This project is almost entirely vibe-coded with [Claude Code](https://claude.ai/claude-code)
