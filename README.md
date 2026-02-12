# Pasta

A simple WPF tool that pastes pre-configured text and images into the target window with one click.

## How it works

1. Click **Open** to open the data folder
2. Edit `text.txt` and put images in the `images` folder
3. Click **Paste** — Pasta will simulate Alt+Tab to switch to the previous window, then simulate Ctrl+V to paste the text and images

> **Note:** Firefox only supports pasting the first image. Use a Chromium-based browser (Chrome, Edge, etc.) for full functionality.

## Requirements

- Windows x64
- .NET 8 Desktop Runtime

## Usage

- **Directly run:** Run `Pasta.exe` directly.
- **Install:** Run `install.bat` to copy binaries to `%LocalAppData%\Pasta` and create shortcuts on the desktop and Start Menu.

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
