using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;

namespace FreeCourse.Services.Catalog.Mapping;

public class GeneralMapping:Profile
{
    public GeneralMapping()
    {
        #region CourseMapping
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<Course, CourseCreateDto>().ReverseMap();
        CreateMap<Course, CourseUpdateDto>().ReverseMap();
        #endregion

        #region CategoryMapping

        CreateMap<Category, CategoryDto>().ReverseMap();

        #endregion

        #region FeatureMapping

        CreateMap<Feature, FeatureDto>().ReverseMap();

        #endregion
    }
}
