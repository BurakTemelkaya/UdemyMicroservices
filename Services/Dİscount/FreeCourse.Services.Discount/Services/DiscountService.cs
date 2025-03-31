using Dapper;
using FreeCourse.Shared.Dtos;
using Npgsql;
using System.Data;

namespace FreeCourse.Services.Discount.Services;

public class DiscountService : IDiscountService
{
    private readonly IConfiguration _configuration;
    private readonly IDbConnection _dbConnection;

    public DiscountService(IConfiguration configuration)
    {
        _configuration = configuration;
        _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
    }

    public async Task<Response<NoContent>> DeleteAsync(int id)
    {
        int deleteStatusCode = await _dbConnection.ExecuteAsync("DELETE FROM discount WHERE id=@Id", new { Id = id });

        if (deleteStatusCode <= 0)
        {
            return Response<NoContent>.Fail("Discount not found.", StatusCodes.Status404NotFound);
        }

        return Response<NoContent>.Success(StatusCodes.Status204NoContent);
    }

    public async Task<Response<List<Models.Discount>>> GetAllAsync()
    {
        IEnumerable<Models.Discount> discount = await _dbConnection.QueryAsync<Models.Discount>("SELECT * FROM discount");

        return Response<List<Models.Discount>>.Success([.. discount], StatusCodes.Status200OK);
    }

    public async Task<Response<Models.Discount>> GetByCodeAndUserIdAsync(string code, string userId)
    {
        IEnumerable<Models.Discount> discount = await _dbConnection.QueryAsync<Models.Discount>("SELECT * FROM discount WHERE code=@Code AND userid=@UserId", new { Code = code, UserId = userId });

        Models.Discount? hasDiscount = discount.FirstOrDefault();

        if (hasDiscount == null)
        {
            return Response<Models.Discount>.Fail("Discount not found", StatusCodes.Status404NotFound);
        }

        return Response<Models.Discount>.Success(hasDiscount, StatusCodes.Status200OK);
    }

    public async Task<Response<Models.Discount>> GetByIdAsync(int id)
    {
        Models.Discount? discount = (await _dbConnection.QueryAsync<Models.Discount>("SELECT * from discount where id=@Id", new { Id = id })).SingleOrDefault();

        if (discount == null)
        {
            return Response<Models.Discount>.Fail("Discount not found", StatusCodes.Status404NotFound);
        }

        return Response<Models.Discount>.Success(discount, StatusCodes.Status200OK);
    }

    public async Task<Response<NoContent>> SaveAsync(Models.Discount discount)
    {
        int saveStatus = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid, rate, code) VALUES (@UserId, @Rate, @Code)", discount);

        if (saveStatus <= 0)
        {
            return Response<NoContent>.Fail("An error occurred while adding the discount", StatusCodes.Status500InternalServerError);
        }

        return Response<NoContent>.Success(StatusCodes.Status201Created);
    }

    public async Task<Response<NoContent>> UpdateAsync(Models.Discount discount)
    {
        int updateStatus = await _dbConnection.ExecuteAsync("UPDATE discount SET userid=@UserId, rate=@Rate, code=@Code WHERE id=@Id",
        new{
            discount.UserId,
            discount.Rate,
            discount.Code,
            discount.Id
        });

        if (updateStatus <= 0)
        {
            return Response<NoContent>.Fail("Discount not found.", StatusCodes.Status404NotFound);
        }

        return Response<NoContent>.Success(StatusCodes.Status204NoContent);
    }
}
