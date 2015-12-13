using FileExplorer.IO;
using FileExplorer.Models;
using FileExplorer.Script;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.UnitTests
{
    public static class ScriptCommandTests
    {
        private static ScriptCommandSerializer serializer = new ScriptCommandSerializer(typeof(FileExplorer3Commands));
        private static IScriptCommand serializeAndDeserializeCommand(IScriptCommand command)
        {
            var stream = serializer.SerializeScriptCommand(command);

            Console.WriteLine(new StreamReader(stream).ReadToEnd());
            stream.Seek(0, SeekOrigin.Begin);

            return serializer.DeserializeScriptCommand(stream);
        }

        public static async Task UnitTest()
        {
            await Test_ParsePath();
            await Test_DiskCreatePath();
            await Test_OpenStream();
            await Test_CopyStream();
        }

        public static async Task IntegrationTest()
        {
            await Test_CopyFile();
            await Test_DownloadFile();
        }

        static string dummyPath = "Test_Path\\Test_File";
        static string dummyDir = PathHelper.Disk.GetDirectoryName(dummyPath);
        static string dummyName = PathHelper.Disk.GetFileName(dummyPath);

        public static async Task Test_ParsePath()
        {
            //Prepare
            ParsePath cmd = new ParsePath();
            var mockProfile = new Mock<IProfile>();
            var mockModel = new Mock<IEntryModel>();

            mockProfile.Setup(p => p.ParseAsync(dummyPath)).Returns(Task.FromResult<IEntryModel>(mockModel.Object));

            ParameterDic pd = new ParameterDic() 
            {
                { cmd.ProfileKey, mockProfile.Object },
                { cmd.PathKey, dummyPath}               
            };

            //Action
            await cmd.ExecuteAsync(pd);

            //Assert
            Assert.IsTrue(pd.ContainsKey(cmd.DestinationKey));
            Assert.AreEqual(mockModel.Object, pd[cmd.DestinationKey]);
        }

        public static async Task Test_DiskCreatePath()
        {
            //Prepare
            DiskCreatePath cmd = new DiskCreatePath();
            var mockProfile = new Mock<IDiskProfile>();
            var mockModel = new Mock<IEntryModel>();

            mockProfile.Setup(p => p.Path).Returns(PathHelper.Disk);
            mockProfile.Setup(p => p.DiskIO.CreateAsync(dummyPath, false, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(() => Task.FromResult(mockModel.Object))
                .Verifiable();

            ParameterDic pd = new ParameterDic() 
            {
                { cmd.ProfileKey, mockProfile.Object },
                { cmd.PathKey, dummyPath}               
            };

            //Action
            await cmd.ExecuteAsync(pd);

            //Assert
            Assert.IsTrue(pd.ContainsKey(cmd.DestinationKey));
            Assert.AreEqual(mockModel.Object, pd[cmd.DestinationKey]);
            mockProfile.Verify();
        }

        public static async Task Test_OpenStream()
        {            
            //Prepare
            var streamCmd = new Mock<ScriptCommandBase>();  
            var thenCmd = new Mock<ScriptCommandBase>();  
            DiskOpenStream openStream = new DiskOpenStream() { NextCommand = streamCmd.Object, ThenCommand = thenCmd.Object };
            var mockProfile = new Mock<IDiskProfile>();
            var mockModel = new Mock<IEntryModel>();

            mockModel.Setup(em => em.Profile).Returns(() => mockProfile.Object);
            mockProfile.Setup(p => p.DiskIO.OpenStreamAsync(mockModel.Object, openStream.Access, 
                It.IsAny<CancellationToken>())).Returns(Task.FromResult<Stream>(new MemoryStream())).Verifiable();

            ParameterDic pd = new ParameterDic() 
            {
                { openStream.EntryKey, mockModel.Object },                              
            };

            streamCmd.Setup(cmd => cmd.CanExecute(It.IsAny<ParameterDic>())).Returns(true);
            streamCmd.Setup(cmd => cmd.ExecuteAsync(It.IsAny<ParameterDic>()))
                .Returns<ParameterDic>(pm => Task.Run(() =>
                {
                    Assert.IsTrue(pm.ContainsKey(openStream.StreamKey));
                    Assert.IsTrue(pm[openStream.StreamKey] is MemoryStream);
                    return (IScriptCommand)ResultCommand.NoError;
                })).Verifiable(); 

            //Action
            IScriptCommand retCmd = await openStream.ExecuteAsync(pd);

            //Assert
            mockProfile.Verify();
            streamCmd.Verify();
            Assert.IsFalse(pd.ContainsKey(openStream.StreamKey));
            Assert.AreEqual(thenCmd.Object, retCmd);
        }

        public static async Task Test_CopyStream()
        {
            //Prepare
            MemoryStream msSource = new MemoryStream();
            StreamWriter swSource = new StreamWriter(msSource);
            swSource.Write(dummyPath);
            swSource.Flush();
            msSource.Seek(0, SeekOrigin.Begin);
            MemoryStream msDest = new MemoryStream();
            StreamReader reader = new StreamReader(msDest);

            CopyStream copyStream = new CopyStream();
            ParameterDic pd = new ParameterDic() 
            {
                { copyStream.SourceKey, msSource },                              
                { copyStream.DestinationKey, msDest }
            };

            //Action
            IScriptCommand retCmd = await copyStream.ExecuteAsync(pd);            
            msDest.Seek(0, SeekOrigin.Begin);
            string read = reader.ReadToEnd();
            
            //Assert
            Assert.AreEqual(dummyPath, read);
        }

        public static async Task Test_CopyFile()
        {
            string tempDirectory = "C:\\Temp";
            string srcFile = System.IO.Path.Combine(tempDirectory, "File1.txt");
            string destFile = System.IO.Path.Combine(tempDirectory, "File2.txt");
            string signature = "Created by testCopyFile at " + DateTime.Now.ToString();
            Directory.CreateDirectory(tempDirectory);
            using (var sw = File.CreateText(srcFile))
                sw.WriteLine(signature);
            //File.Delete(destFile);

            IProfile fsiProfile = new FileSystemInfoExProfile(null, null);


            IScriptCommand copyCommand =
               CoreScriptCommands.ParsePath("{Profile}", "{SourceFile}", "{Source}",
               CoreScriptCommands.DiskParseOrCreateFile("{Profile}", "{DestinationFile}", "{Destination}",
               CoreScriptCommands.DiskOpenStream("{Source}", "{SourceStream}", FileExplorer.Defines.FileAccess.Read,
               CoreScriptCommands.DiskOpenStream("{Destination}", "{DestinationStream}", FileExplorer.Defines.FileAccess.Write,
               CoreScriptCommands.CopyStream("{SourceStream}", "{DestinationStream}"))))
               , ResultCommand.Error(new FileNotFoundException(srcFile))
                );

            //copyCommand = ScriptCommands.CopyFile("SourceFile", "DestinationFile");
            copyCommand = serializeAndDeserializeCommand(copyCommand);
            await ScriptRunner.RunScriptAsync(new ParameterDic() { 
                { "Profile", fsiProfile },
                { "SourceFile", srcFile },
                { "DestinationFile", destFile}
            }, copyCommand);


            string actual = null;
            using (var sr = File.OpenText(destFile))
            {
                actual = sr.ReadLine();
            }

            Assert.AreEqual(signature, actual);
        }

        public static async Task Test_DownloadFile()
        {
            string downloadUrl = "http://www.quickzip.org/logo2.png";
            string tempDirectory = "C:\\Temp";
            string destFile = System.IO.Path.Combine(tempDirectory, "output.png");            

            IScriptCommand downloadCommand =
              CoreScriptCommands.Download("Url", "Stream",
                CoreScriptCommands.DiskParseOrCreateFile("{Profile}", "{DestinationFile}", "{Destination}",
                CoreScriptCommands.DiskOpenStream("{Destination}", "{DestinationStream}", FileExplorer.Defines.FileAccess.Write,
                CoreScriptCommands.CopyStream("{Stream}", "{DestinationStream}"))));
            
            //downloadCommand = ScriptCommands.DownloadFile("Url", "DestinationFile");

            downloadCommand = serializeAndDeserializeCommand(downloadCommand);
            await ScriptRunner.RunScriptAsync(new ParameterDic() { 
                { "Profile", new FileSystemInfoExProfile(null, null) },
                { "Url", downloadUrl },
                { "DestinationFile", destFile },                
            }, downloadCommand);

        }

    }
}
