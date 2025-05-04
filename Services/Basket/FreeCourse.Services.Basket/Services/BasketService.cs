using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared.Dtos;
using StackExchange.Redis;
using System.Text.Json;

namespace FreeCourse.Services.Basket.Services;

public class BasketService : IBasketService
{
    private readonly RedisService _redisService;
    public BasketService(RedisService redisService)
    {
        _redisService = redisService;
    }

    public async Task<Response<BasketDto>> GetBasketAsync(string userId)
    {
        RedisValue existBasket = await _redisService.GetDb().StringGetAsync(userId);
        if (existBasket.IsNullOrEmpty)
        {
            return Response<BasketDto>.Fail("Basket not found", StatusCodes.Status404NotFound);
        }

        return Response<BasketDto>.Success(JsonSerializer.Deserialize<BasketDto>(existBasket!)!, StatusCodes.Status200OK);
    }
    public async Task<Response<bool>> SaveOrUpdateAsync(BasketDto basketDto)
    {
        bool status = await _redisService.GetDb().StringSetAsync(basketDto.UserId, JsonSerializer.Serialize(basketDto));

        if (!status)
        {
            return Response<bool>.Fail("Basket could not update or save", StatusCodes.Status500InternalServerError);
        }

        return Response<bool>.Success(status, StatusCodes.Status204NoContent);
    }

    public async Task<Response<bool>> DeleteAsync(string userId)
    {
        bool status = await _redisService.GetDb().KeyDeleteAsync(userId);

        if (!status)
        {
            return Response<bool>.Fail("Basket not found", StatusCodes.Status404NotFound);
        }

        return Response<bool>.Success(status, StatusCodes.Status204NoContent);
    }

    public async Task<Response<ICollection<BasketDto>>> GetAllBasketAsync()
    {
        var data = await _redisService.GetAllKeyValuesAsync();

        if (data == null || data.Count == 0)
        {
            return Response<ICollection<BasketDto>>.Fail("Basket not found", StatusCodes.Status404NotFound);
        }

        var basketList = data.Values
            .Select(json => JsonSerializer.Deserialize<BasketDto>(json))
            .Where(dto => dto != null)
            .ToList();

        return Response<ICollection<BasketDto>>.Success(basketList!, StatusCodes.Status200OK);
    }

}
