using System.Diagnostics.CodeAnalysis;
using Bogus;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Builders;

public sealed class ImageBuilder(Faker faker, DateTime nowUtc)
{
    private Description? _description;
    private Name? _name;
    private Uri? _uri;

    public Image Build()
    {
        AsignValuesToNullProperties();
        Image image = Image.Create(nowUtc, _uri, _name, _description).Value;
        return image;
    }

    public ImageBuilder WithUri(Uri uri)
    {
        _uri = uri;
        return this;
    }

    public ImageBuilder WithName(Name name)
    {
        _name = name;
        return this;
    }

    public ImageBuilder WithDescription(Description description)
    {
        _description = description;
        return this;
    }

    [MemberNotNull(nameof(_uri))]
    [MemberNotNull(nameof(_name))]
    [MemberNotNull(nameof(_description))]
    private void AsignValuesToNullProperties()
    {
        _uri ??= new Uri(faker.Internet.UrlWithPath());
        _name ??= new Name(faker.Name.FirstName());
        _description ??= new Description(faker.Name.LastName());
    }
}
