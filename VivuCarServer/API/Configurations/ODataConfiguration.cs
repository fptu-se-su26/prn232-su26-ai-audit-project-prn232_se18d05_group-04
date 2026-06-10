using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace API.Configurations;

public static class ODataConfiguration
{
    public static IMvcBuilder AddVivuCarOData(this IMvcBuilder mvcBuilder)
    {
        var modelBuilder = new ODataConventionModelBuilder();

        // Add EntitySet registrations here after domain models are scaffolded.
        return mvcBuilder.AddOData(options =>
            options
                .Select()
                .Filter()
                .OrderBy()
                .Expand()
                .Count()
                .SetMaxTop(100)
                .AddRouteComponents("odata", modelBuilder.GetEdmModel())
        );
    }
}
