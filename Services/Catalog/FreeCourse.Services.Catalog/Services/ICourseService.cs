﻿using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Services.Catalog.Services;

public interface ICourseService
{
    Task<Response<List<CourseDto>>> GetAllAsync();
    Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto);
    Task<Response<CourseDto>> GetByIdAsync(string id);
    Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto);
    Task<Response<NoContent>> DeleteAsync(string id);
    Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId);
}
