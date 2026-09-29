## Payemnt Service WEB API

# Payment API Test Cases

 ## 1\. Create Payment — Success

 ### Request

 **POST**

```
/api/Payment
```

 ### Request Body

```
{
  "orderId": 1,
  "amount": 1500.00
}
```

 ### Expected Response

 **HTTP 200 OK**

```
{
  "id": 1,
  "orderId": 1,
  "amount": 1500.00,
  "status": "Paid",
  "createdAt": "..."
}
```

 ### Verify in SQL Server

```
SELECT *
FROM Payments;
```

 You should see the newly created payment in the `Payments` table.

---

 ## 2\. Create Another Payment

 ### Request

 **POST**

```
/api/Payment
```

 ### Request Body

```
{
  "orderId": 2,
  "amount": 2500.50
}
```

 ### Expected Response

 **HTTP 200 OK**

 The response should contain:

```
OrderId = 2
Amount  = 2500.50
Status  = Paid
```

---

 ## 3\. Get Existing Payment

 ### Request

 **GET**

```
/api/Payment/order/1
```

 ### Expected Response

 **HTTP 200 OK**

 The response should contain the payment belonging to **Order 1**.

---

 ## 4\. Get Payment That Doesn't Exist

 Use an order ID that has no payment.

 ### Request

 **GET**

```
/api/Payment/order/9999
```

 ### Expected Response

 **HTTP 404 Not Found**

```
{
  "success": false,
  "message": "Payment for Order 9999 was not found."
}
```

---

 ## 5\. Important Test — Decimal Amount

 This test confirms that decimal amounts are mapped correctly.

 ### Request

 **POST**

```
/api/Payment
```

 ### Request Body

```
{
  "orderId": 3,
  "amount": 999.99
}
```

 ### Expected Response

 **HTTP 200 OK**

 The response should contain:

```
Amount = 999.99
Status = Paid
```

 This confirms that the decimal amount mapping is working correctly.