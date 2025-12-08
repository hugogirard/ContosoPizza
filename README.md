![Agent Diagram](diagrams/agent.png)

# Architecture

### Orchestrator Agent (Main Router)

**Role**:

- Receives all user messages

- Detects intent

- Routes to the correct agent

- Merges responses

- Provides final answer back to frontend

**Example decisions**:

- “What pizzas do you have?” → route to Menu Agent

- “Add 2 pepperoni large to my basket” → Menu Agent

- “Checkout please” → Order & Payment Agent

- “Pay with Visa” → Order & Payment Agent

This orchestrator demonstrates true agentic reasoning.

### Menu & Basket Agent (MCP Tool Agent)

**Purpose:**

- Only interacts with your MCP tool

- Reads menu, toppings, sizes

- Maintains a local basket/cart representation

- Can return structured objects back to the orchestrator

This agent demonstrates **tool-use with structured local data.**

**Exposed Tool**

```
get_menu()
add_to_basket(item, qty)
remove_from_basket(item)
get_basket()
```

### Order & Payment Agent (External API Agent)

Purpose:

- Talks to a REST API

- Handles order submission & payment

- Works only AFTER the orchestrator sends it a **basket summary**

**API endpoints this agent would call:**

- POST /orders → submit the final basket
- POST /payments → run a credit card
- GET /orders/{id} → check status

