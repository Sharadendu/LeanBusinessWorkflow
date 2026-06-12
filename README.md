# LeanBusinessWorkflow

A modern .NET 10 solution combining **Semantic Kernel**, **Obsidian-style knowledge management**, and a **Blazor AI Chat** interface.

## Solution Architecture

```
LeanBusinessWorkflow/
├── src/
│   ├── LeanBusinessWorkflow.Core/          # Shared domain models & Obsidian vault service
│   ├── LeanBusinessWorkflow.AI/            # Semantic Kernel chat service + DI extensions
│   ├── LeanBusinessWorkflow.Web/           # Blazor Web App (AI Chat UI)
│   └── LeanBusinessWorkflow.Console/       # Console client (uses shared libraries)
```

### Projects

| Project                        | Type              | Purpose                                      |
|--------------------------------|-------------------|----------------------------------------------|
| `LeanBusinessWorkflow.Core`    | Class Library     | `Note` model + `ObsidianVaultService`        |
| `LeanBusinessWorkflow.AI`      | Class Library     | `ChatService` (Semantic Kernel) + DI helpers |
| `LeanBusinessWorkflow.Web`     | Blazor Web App    | Interactive AI Chat interface                |
| `LeanBusinessWorkflow.Console` | Console App       | CLI client using shared services             |

## Features

- **Obsidian-style Notes**: Markdown files with YAML frontmatter stored in a local vault
- **Semantic Kernel Integration**: Powered AI chat with OpenAI or Azure OpenAI
- **Shared Libraries**: Both Web and Console apps leverage the same `Core` and `AI` projects
- **Microsoft.Extensions**: Clean dependency injection and configuration

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- OpenAI or Azure OpenAI API key

### Configuration

Update `src/LeanBusinessWorkflow.Web/appsettings.json`:

```json
{
  "AI": {
    "ModelId": "gpt-4o-mini",
    "UseAzureOpenAI": false,
    "ApiKey": "sk-..."
  },
  "Obsidian": {
    "VaultPath": "Data/Vault"
  }
}
```

> **Security Tip**: Use User Secrets or environment variables for production API keys.

### Running the Application

**Blazor Web App (Recommended)**

```bash
dotnet run --project src/LeanBusinessWorkflow.Web
```

Then navigate to `https://localhost:xxxx/chat`

**Console App**

```bash
dotnet run --project src/LeanBusinessWorkflow.Console
```

## Project References

- `Core` → Referenced by `AI`, `Web`, and `Console`
- `AI` → Referenced by `Web` and `Console`

## Technologies

- **.NET 10**
- **Semantic Kernel** 1.48+
- **Blazor** (Interactive Server)
- **YamlDotNet** (for frontmatter parsing)
- **Microsoft.Extensions** (DI, Configuration, Logging)

## License

This project was created as part of the LeanBusinessWorkflow initiative.