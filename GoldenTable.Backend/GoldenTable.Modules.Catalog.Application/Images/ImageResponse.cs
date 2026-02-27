using GoldenTable.Modules.Catalog.Domain.Common.Image;

namespace GoldenTable.Modules.Catalog.Application.Images;

public sealed class ImageResponse
{
    public ImageResponse(Image image)
    {
        Uri = image.Uri.AbsoluteUri;
        Name = image.Name.Value;
        Description = image.Description is null ? string.Empty : image.Description.Value;
    }

    public string Uri { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
