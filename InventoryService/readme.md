# Inventory Service

## Build and Run

1. Build the project.
2. Run **InventoryService**.
3. Open Swagger UI to access the available endpoints:
   * `GET /api/Inventory`
   * `GET /api/Inventory/{productId}`

---

## Testing Endpoints -> `https://localhost:7114/api/Inventory/reserve-batch`

### 1. Test Get All Inventory
* **Method:** `GET`
* **URL:** `/api/Inventory`

#### Expected Result
```json
[
  {
    "productId": 1,
    "stockQuantity": 10
  },
  {
    "productId": 2,
    "stockQuantity": 5
  },
  {
    "productId": 3,
    "stockQuantity": 0
  },
  {
    "productId": 4,
    "stockQuantity": 20
  }
]

```

---

````
# Stock Reservation API — Test Cases

This document covers the expected responses for the stock reservation endpoint.

## 1. ✅ Sufficient Stock → `200 OK`

Use a product that currently has enough stock, for example **Product 1**.

### Request

```json
{
  "items": [
    {
      "productId": 1,
      "quantity": 1
    }
  ]
}
````

 ### Expected Response

```
{
  "success": true,
  "message": "Stock reserved successfully."
}
```

 **HTTP Status:** `200 OK`

---

 ## 2\. ❌ Product Doesn't Exist → `404 Not Found`

 Use a product ID that doesn't exist, for example **9999**.

 ### Request

```
{
  "items": [
    {
      "productId": 9999,
      "quantity": 1
    }
  ]
}
```

 ### Expected Response

```
{
  "success": false,
  "message": "Product 9999 was not found."
}
```

 **HTTP Status:** `404 Not Found`

---

 ## 3\. ❌ Insufficient Stock → `409 Conflict`

 Use **Product 3**, which previously had `0` stock.

 ### Request

```
{
  "items": [
    {
      "productId": 3,
      "quantity": 1
    }
  ]
}
```

 ### Expected Response

```
{
  "success": false,
  "message": "Insufficient stock for Product 3."
}
```

 **HTTP Status:** `409 Conflict`

```

```