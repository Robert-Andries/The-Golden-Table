using GoldenTable.Modules.Catalog.Domain.Common.Image;

namespace GoldenTable.Modules.Catalog.Application.Images;

public sealed class ImageResponse
{
    public ImageResponse(Image image)
    {
        Id = image.Id;
        Uri = image.Uri.ToString();
        Name = image.Name.Value;
        Description = image.Description is null ? string.Empty : image.Description.Value;
    }

    public Guid Id { get; set; }
    public string Uri { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
