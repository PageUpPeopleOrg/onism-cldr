﻿using PageUp.CldrPackager.Tools;
using PageUp.CldrPackager.Tools.Subsetting;
using Xunit;

namespace PageUp.CldrPackager.Test
{
    public class CldrDataVersionTest
    {
        [Fact]
        public void BuilderParsesVersionFromCldrSourcesTest()
        {
            var patterns = new PatternCollectionBuilder().Build();
            var builder = new CldrDataBuilder();

            var data = builder.Build(TestSettings.GetCldrFileInputDirectoryPath(), patterns);

            Assert.NotNull(data.PageUpVersion);
            Assert.NotEmpty(data.PageUpVersion);
        }
    }
}
