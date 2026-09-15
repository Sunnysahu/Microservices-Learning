# Order API Documentation

This documentation outlines the endpoints available for managing orders in the application.

## Base URL
`https://localhost:7006/api/Order`

---

## Endpoints

### 1. Add a New Order
Creates a new order with the specified items and customer details.

* **URL:** `/api/Order`
* **Method:** `POST`
* **Content-Type:** `application/json`

#### Request Body
```json
{
    "id": 3,
    "customerName": "Sunny",
    "status": "Pending",
    "createdAt": "2026-09-15T14:02:13.8905347",
    "items": [
        {
            "productId": 1,
            "quantity": 2
        },
        {
            "productId": 2,
            "quantity": 1
        }
    ]
}

```

---

### 2. Get Order by ID

Retrieves the details of a specific order by its unique identifier. Use this endpoint after creating a new order to verify its creation.

* **URL:** `/api/Order/{id}`
* **Method:** `GET`
* **Example URL:** `https://localhost:7006/api/Order/3`

---

## Quick Test with cURL

### Add Order

```bash
curl -k -X POST https://localhost:7006/api/Order \
-H "Content-Type: application/json" \
-d '{
    "id": 3,
    "customerName": "Sunny",
    "status": "Pending",
    "createdAt": "2026-09-15T14:02:13.8905347",
    "items": [
        {
            "productId": 1,
            "quantity": 2
        },
        {
            "productId": 2,
            "quantity": 1
        }
    ]
}'

```

### Get Order

```bash
curl -k -X GET https://localhost:7006/api/Order/3

```

*(Note: The `-k` flag is used to bypass self-signed SSL certificate warnings for local HTTPS development).*