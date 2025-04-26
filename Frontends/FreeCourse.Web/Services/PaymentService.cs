using FreeCourse.Web.Models.FakePayment;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;

    public PaymentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ReceivePaymentAsync(PaymentInfoInput paymentInfoInput)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync<PaymentInfoInput>("fakepayment", paymentInfoInput);

        return response.IsSuccessStatusCode;
    }
}
