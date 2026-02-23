using Bogus;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Tests.Images;

#pragma warning disable CA1515
public abstract class ImageBaseTest : BaseTest
#pragma warning restore CA1515
{
    protected ImageBaseTest()
    {
        SometimeUtc = DateTime.UtcNow;
        ImageFaker = new Faker<Image>().CustomInstantiator(f =>
        {
            Name name = new(f.Name.FullName());
            Description description = new(f.Name.JobDescriptor());
            Uri uri = new(f.Internet.Url());
            Image image = Image.Create(SometimeUtc, uri, name, description).Value;
            image.ClearDomainEvents();
            return image;
        });
    }

    protected Faker<Image> ImageFaker { get; init; }
    protected DateTime SometimeUtc { get; init; }
}
