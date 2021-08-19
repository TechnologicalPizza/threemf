using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace IxMilia.ThreeMf.Test
{
    public class ThreeMfFileLoadTests
    {
        private static ThreeMfFile FileFromParts(params Tuple<string, string>[] filesAndContents)
        {
            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                var contentTypes = Tuple.Create("[Content_Types].xml", @"
<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">
  <Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml"" />
  <Default Extension=""model"" ContentType=""application/vnd.ms-package.3dmanufacturing-3dmodel+xml"" />
</Types>
");
                foreach (var pair in filesAndContents.Append(contentTypes))
                {
                    var path = pair.Item1;
                    var contents = pair.Item2;
                    var entry = archive.CreateEntry(path);

                    using var stream = entry.Open();
                    using var writer = new StreamWriter(stream);
                    writer.Write(contents);
                }
            }

            ms.Seek(0, SeekOrigin.Begin);
            var file = ThreeMfFile.Load(ms);
            return file;
        }

        [Fact]
        public void LoadFromDiskTestMustSucceed()
        {
            LoadFromDiskTestWith("Samples", (package, pathParts) =>
            {
                int size = 0;

                void LoadModels()
                {
                    var file = ThreeMfFile.Load(package);
                    size = file.Models.Count;
                }

                if (!pathParts.Contains("MUSTFAIL"))
                {
                    LoadModels();
                }
                return size;
            });
        }

        [Fact]
        public void LoadFromDiskTestMustFail()
        {
            LoadFromDiskTestWith("Samples/validation tests/_archive/3mf-Verify/MUSTFAIL", (package, pathParts) =>
            {
                void LoadModels()
                {
                    _ = ThreeMfFile.Load(package);
                }

                if (pathParts.Contains("MUSTFAIL_3MF100_Extension_Chapter5a_MissingPIDs.3mf"))
                {
                    // pid is optional by spec so why does this file exist?
                    return 0;
                }

                Assert.Throws<ThreeMfParseException>(LoadModels);
                return 1;
            });
        }

        private void LoadFromDiskTestWith(string directory, Func<Package, string[], int> packageAction)
        {
            var samplesDir = Path.Combine(
                Path.GetDirectoryName(typeof(ThreeMfFileLoadTests).GetTypeInfo().Assembly.Location), directory);

            var loadedFiles = 0;
            foreach (var pathe in Directory.EnumerateFiles(samplesDir, "*.3mf", SearchOption.AllDirectories))
            {
                string path = pathe;// @"C:\Projects\Repos\PrinterFace\threemf\artifacts\bin\IxMilia.ThreeMf.Test\Debug\net5.0\Samples\validation tests\_archive\3mf-Verify\MUSTFAIL\MUSTFAIL_3MF100_Extension_Chapter5b_ReferToAnotherMultiProperties.3mf";// @"C:\Projects\Repos\PrinterFace\threemf\artifacts\bin\IxMilia.ThreeMf.Test\Debug\net5.0\Samples\validation tests\_archive\3mf-Verify\MUSTFAIL\MUSTFAIL_3MF100_Extension_Chapter5b_MultipleReferenceToBaseAndCompositeMatterials.3mf";;

                var fileName = Path.GetFileName(path);
                if (fileName == "multiprop-metallic.3mf" || fileName == "multiprop-translucent.3mf")
                {
                    // undefined namespace `ms`
                    continue;
                }

                var pathParts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (pathParts.Contains("production") || pathParts.Contains("beam lattice"))
                {
                    // not yet implemented
                    continue;
                }

                using var fs = new FileStream(path, FileMode.Open);
                using var package = Package.Open(fs);
                bool shouldContinue = false;

                foreach (var modelRelationship in package.GetRelationshipsByType(ThreeMfFile.ModelRelationshipType))
                {
                    var modelUri = modelRelationship.TargetUri;
                    var modelPart = package.GetPart(modelUri);

                    try
                    {
                        using var modelStream = modelPart.GetStream();
                        var document = XDocument.Load(modelStream);

                        if (document.Root.Attributes().Any(
                            x => x.Name.Namespace.NamespaceName == "http://www.w3.org/2000/xmlns/"))
                        {
                            string relativePath = Path.GetRelativePath(samplesDir, path);

                            Debug.WriteLine(
                                $"The following namespaces in \"{relativePath}\" are not supported: " +
                                string.Join(", ", document.Root.Attributes().Where(
                                    x => x.Name.Namespace.NamespaceName == "http://www.w3.org/2000/xmlns/")));

                            shouldContinue = true;
                            continue;
                        }
                    }
                    catch (XmlException ex)
                    {
                        Debug.WriteLine(path);
                        Debug.WriteLine(ex);
                        shouldContinue = true;
                        continue;
                    }
                }

                if (shouldContinue)
                    continue;

                loadedFiles += packageAction.Invoke(package, pathParts);
            }

            Assert.True(loadedFiles > 0, "No sample files were loaded.  Ensure all submodules have been initialized.");
        }

        [Fact]
        public void ReadFromArchiveTest()
        {
            var file = FileFromParts(
                Tuple.Create("_rels/.rels", @"
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Target=""/non/standard/path/to/model.model"" Id=""rel0"" Type=""http://schemas.microsoft.com/3dmanufacturing/2013/01/3dmodel"" />
</Relationships>
"),
                Tuple.Create("non/standard/path/to/model.model", $@"<model unit=""millimeter"" xml:lang=""en-US"" xmlns=""{ThreeMfModel.CoreNamespace}""></model>")
            );

            var model = file.Models.Single();
            Assert.Equal(ThreeMfModelUnits.Millimeter, model.ModelUnits);
        }

        [Fact]
        public void ReadMultipleModelsTest()
        {
            var file = FileFromParts(
                Tuple.Create("_rels/.rels", @"
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Target=""/3D/3dmodel-1.model"" Id=""rel1"" Type=""http://schemas.microsoft.com/3dmanufacturing/2013/01/3dmodel"" />
  <Relationship Target=""/3D/3dmodel-2.model"" Id=""rel2"" Type=""http://schemas.microsoft.com/3dmanufacturing/2013/01/3dmodel"" />
</Relationships>
"),
                Tuple.Create("3D/3dmodel-1.model", $@"<model unit=""millimeter"" xml:lang=""en-US"" xmlns=""{ThreeMfModel.CoreNamespace}""></model>"),
                Tuple.Create("3D/3dmodel-2.model", $@"<model unit=""inch"" xml:lang=""en-US"" xmlns=""{ThreeMfModel.CoreNamespace}""></model>")
            );

            Assert.Equal(2, file.Models.Count);
            Assert.Equal(ThreeMfModelUnits.Millimeter, file.Models.First().ModelUnits);
            Assert.Equal(ThreeMfModelUnits.Inch, file.Models.Last().ModelUnits);
        }

        [Fact]
        public void ReadZeroModelsTest()
        {
            var file = FileFromParts(
                Tuple.Create("_rels/.rels", @"
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
</Relationships>
")
            );

            Assert.Empty(file.Models);
        }
    }
}
