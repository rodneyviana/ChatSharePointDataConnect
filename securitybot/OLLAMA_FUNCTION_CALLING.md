# Ollama Function Calling Setup

## Important Changes

This project has been updated to support function calling with Ollama by using the OpenAI-compatible endpoint.

### Why the change?

The `Microsoft.SemanticKernel.Connectors.Ollama` package (alpha version) does **not support function calling** in C# yet. According to Microsoft's documentation:

- ? Ollama + ToolCallBehavior: Not supported
- ?? Ollama + FunctionChoiceBehavior: Coming soon (C#)
- ? OpenAI connector with Ollama's OpenAI-compatible endpoint: **Fully supported**

### What changed?

1. **NuGet Package**: Replaced `Microsoft.SemanticKernel.Connectors.Ollama` (alpha) with stable `Microsoft.SemanticKernel.Connectors.OpenAI`
2. **Connection Method**: Using `AddOpenAIChatCompletion()` instead of `AddOllamaChatCompletion()`
3. **Endpoint Format**: Must use `http://localhost:11434/v1` (note the `/v1` suffix)
4. **Function Calling**: Using `FunctionChoiceBehavior.Auto()` instead of deprecated `ToolCallBehavior.AutoInvokeKernelFunctions`

### Environment Variables

Update your `.env` file with:

```env
# Model name (must support function calling)
MODELID="llama3.2"

# OpenAI-compatible endpoint (note the /v1 suffix)
ENDPOINT="http://localhost:11434/v1"

# API key (any value works, Ollama doesn't check)
APIKEY="ollama"

CONNECTIONSTRING="your_connection_string_here"
```

### Supported Models

?? **Not all Ollama models support function calling!** 

According to Microsoft documentation, these models are known to work:
- ? `llama3.2`
- ? `qwen2.5` (or `qwen3:4b`)
- ? Other models that support tool calling

To check if your model supports function calling, consult Ollama's model documentation.

### Testing Function Calling

Your `SqlQueryPlugin` with the `run_sql_query` function should now work correctly. The AI model will:
1. Recognize when a SQL query is needed
2. Call the `run_sql_query` function with appropriate parameters
3. Receive the JSON results
4. Format a response for the user

### Troubleshooting

If function calling still doesn't work:

1. **Verify endpoint**: Make sure you're using `http://localhost:11434/v1` (with `/v1`)
2. **Check model**: Ensure your model supports function calling
3. **Test Ollama**: Run `curl http://localhost:11434/v1/models` to verify the endpoint is working
4. **Review logs**: Check console output for function invocation messages

### Benefits of This Approach

? **Stable**: Using a non-alpha, production-ready package  
? **Standard**: Works with OpenAI-compatible endpoints  
? **Flexible**: Easy to switch between Ollama and Azure OpenAI  
? **Supported**: Full function calling capabilities  
