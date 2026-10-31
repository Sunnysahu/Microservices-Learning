namespace OrderService.Services.Payment.DTOs;

public class PaymentClientResult
{
    public int StatusCode { get; set; }

    public PaymentResponse? Payment { get; set; }

    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;
}