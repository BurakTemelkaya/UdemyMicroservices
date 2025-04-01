using AutoMapper;

namespace FreeCourse.Services.Order.Application.Mapping;

public static class ObjectMapper
{
    private static readonly Lazy<IMapper> lazy = new(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CustomMapping>();
        });
        return config.CreateMapper();
    });

    public static IMapper Mapper => lazy.Value;

}
