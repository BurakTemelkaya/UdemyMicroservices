using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services;

public class CategoryService : ICategoryService
{
    private readonly IMongoCollection<Category> _categoryCollection;
    private readonly IMapper _mapper;
    public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
    {
        MongoClient client = new(databaseSettings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(databaseSettings.DatabaseName);
        _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
        _mapper = mapper;
    }
    public async Task<Response<List<CategoryDto>>> GetAllAsync()
    {
        List<Category> categories = await _categoryCollection.Find(category => true).ToListAsync();
        return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), StatusCodes.Status200OK);
    }
    public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
    {
        Category category = _mapper.Map<Category>(categoryDto);

        await _categoryCollection.InsertOneAsync(category);

        return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), StatusCodes.Status201Created);
    }
    public async Task<Response<CategoryDto>> GetByIdAsync(string id)
    {
        Category? category = await _categoryCollection.Find<Category>(x => x.Id == id).FirstOrDefaultAsync();

        if (category == null)
        {
            return Response<CategoryDto>.Fail("Category not found", StatusCodes.Status404NotFound);
        }

        return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), StatusCodes.Status200OK);
    }
    public async Task<Response<NoContent>> UpdateAsync(CategoryDto categoryDto)
    {
        Category category = _mapper.Map<Category>(categoryDto);

        Category result = await _categoryCollection.FindOneAndReplaceAsync(x => x.Id == category.Id, category);

        return Response<NoContent>.Success(StatusCodes.Status200OK);
    }
    public async Task<Response<NoContent>> DeleteAsync(string id)
    {
        DeleteResult result = await _categoryCollection.DeleteOneAsync(x => x.Id == id);

        if (result.DeletedCount == 0)
        {
            return Response<NoContent>.Fail("Category not found", StatusCodes.Status404NotFound);
        }

        return Response<NoContent>.Success(StatusCodes.Status204NoContent);
    }
}
