# Inventory Service

## Build and Run

Now build the project again.

If successful, run **InventoryService**.

Open Swagger UI.

You should see:

- `GET /api/Inventory`
- `GET /api/Inventory/{productId}`

## Test GET /api/Inventory

Execute:

`GET /api/Inventory`

Expected result:

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
````

 ## Test Product 1

 Execute:

 `GET /api/Inventory/1`

 Expected:

```
{
  "productId": 1,
  "stockQuantity": 10
}
```

 ## Test Product 3

 It is important to test a product with zero stock.

 Execute:

 `GET /api/Inventory/3`

 Expected:

```
{
  "productId": 3,
  "stockQuantity": 0
}
```

 This will be useful later when the Order Service checks whether stock is available.

 ## Test a Nonexistent Product

 Try:

 `GET /api/Inventory/9999`

 Expected:

 `404 Not Found`

 We're returning a basic `NotFound()` for now.

 Later, we'll introduce our consistent response format and global exception handling.


 Please build → run → test all four cases:

- `GET /api/Inventory`
- `GET /api/Inventory/1`
- `GET /api/Inventory/3`
- `GET /api/Inventory/9999`

 Make sure all four cases work as expected before continuing.