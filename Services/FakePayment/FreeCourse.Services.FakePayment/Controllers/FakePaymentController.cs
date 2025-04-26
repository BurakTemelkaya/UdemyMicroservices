using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FakePaymentController : CustomBaseController
{
    [HttpPost]
    public IActionResult ReceivePayment(PaymentDto paymentDto)
    {
        //PaymentDto ile ödeme işlemi gerçekleştir
        return CreateActionResultInstance(Response<NoContent>.Success(StatusCodes.Status204NoContent));
    }
}
