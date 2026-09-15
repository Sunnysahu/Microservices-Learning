# Inventory Service

## Build and Run

1. Build the project.
2. Run **InventoryService**.
3. Open Swagger UI to access the available endpoints:
   * `GET /api/Inventory`
   * `GET /api/Inventory/{productId}`

---

## Testing Endpoints

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

### 2. Test Product 1

* **Method:** `GET`
* **URL:** `/api/Inventory/1`

#### Expected Result

```json
{
  "productId": 1,
  "stockQuantity": 10
}

```

---

### 3. Test Product 3 (Zero Stock)

> **Note:** It is important to test a product with zero stock, as this will be useful later when the Order Service checks whether stock is available.

* **Method:** `GET`
* **URL:** `/api/Inventory/3`

#### Expected Result

```json
{
  "productId": 3,
  "stockQuantity": 0
}

```

---

### 4. Test a Nonexistent Product

* **Method:** `GET`
* **URL:** `/api/Inventory/9999`

#### Expected Result

* **Status Code:** `404 Not Found`
* *(Note: A basic `NotFound()` is returned for now. Consistent response formats and global exception handling will be introduced later).*

---

> **Checklist:** Make sure to build, run, and verify all four test cases work as expected before continuing.