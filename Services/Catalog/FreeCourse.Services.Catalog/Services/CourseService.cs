using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using Mass = MassTransit;
using MongoDB.Driver;
using FreeCourse.Shared.Messages;

namespace FreeCourse.Services.Catalog.Services;

public class CourseService : ICourseService
{
    private readonly IMongoCollection<Course> _courseCollection;
    private readonly IMongoCollection<Category> _categoryCollection;
    private readonly IMapper _mapper;
    private readonly Mass.IPublishEndpoint _publishEndpoint;

    public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, Mass.IPublishEndpoint publishEndpoint)
    {
        MongoClient client = new(databaseSettings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(databaseSettings.DatabaseName);
        _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
        _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }
    public async Task<Response<List<CourseDto>>> GetAllAsync()
    {
        List<Course> courses = await _courseCollection.Find(course => true).ToListAsync();

        if (courses.Count != 0)
        {
            foreach (Course course in courses)
            {
                course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
            }
        }
        else
        {
            courses = [];
        }

        return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), StatusCodes.Status200OK);
    }
    public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
    {
        Course newCourse = _mapper.Map<Course>(courseCreateDto);

        newCourse.CreatedTime = DateTime.Now;

        await _courseCollection.InsertOneAsync(newCourse);

        return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), StatusCodes.Status201Created);
    }
    public async Task<Response<CourseDto>> GetByIdAsync(string id)
    {
        Course? course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

        if (course == null)
        {
            return Response<CourseDto>.Fail("Course not found", StatusCodes.Status404NotFound);
        }

        course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();

        return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), StatusCodes.Status200OK);
    }

    public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
    {
        List<Course> courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();
        if (courses.Count != 0)
        {
            foreach (Course course in courses)
            {
                course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
            }
        }
        else
        {
            courses = [];
        }
        return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), StatusCodes.Status200OK);
    }

    public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
    {
        Course course = _mapper.Map<Course>(courseUpdateDto);

        Course? result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == course.Id, course);

        if (result == null)
        {
            return Response<NoContent>.Fail("Course not found", StatusCodes.Status404NotFound);
        }

        await _publishEndpoint.Publish(new CourseNameChangedEvent() { CourseId = course.Id, UpdatedName = course.Name });

        return Response<NoContent>.Success(StatusCodes.Status204NoContent);
    }

    public async Task<Response<NoContent>> DeleteAsync(string id)
    {
        DeleteResult result = await _courseCollection.DeleteOneAsync(x => x.Id == id);

        if (result.DeletedCount == 0)
        {
            return Response<NoContent>.Fail("Course not found", StatusCodes.Status404NotFound);
        }

        return Response<NoContent>.Success(StatusCodes.Status204NoContent);
    }
}
