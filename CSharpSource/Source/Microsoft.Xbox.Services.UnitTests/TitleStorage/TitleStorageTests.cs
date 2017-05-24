// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.UnitTests.TitleStorage
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Text;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Shared.TitleStorage;
    using Microsoft.Xbox.Services.TitleStorage;

    using Newtonsoft.Json.Linq;

    [TestClass]
    public class TitleStorageTests: TestBase
    {
        private TitleStorageService titleStorageService;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            MockXboxLiveData.Load(Environment.CurrentDirectory + "\\TitleStorage\\TitleStorageUT.json");
            this.titleStorageService = new TitleStorageService();
        }

        [TestMethod]
        public async Task TestGetQuotaAsyncWithGlobalStorage()
        {
            // Arrange
            var urlExpected = @"https://titlestorage.xboxlive.com/global/scids/00000000-0000-0000-0000-0000694f5acb";
            MockXboxLiveData.MockRequestData mockRequestData = MockXboxLiveData.MockResponses["GlobalStorageGetQuota"];
            var responseJson = JObject.Parse(mockRequestData.Response.ResponseBodyString);

            // Act
            var titleStorageQuota = await this.titleStorageService.GetQuotaAsync(user, TitleStorageType.GlobalStorage);

            // Assert
            Assert.AreEqual("GET", mockRequestData.Request.Method);
            Assert.AreEqual(urlExpected, mockRequestData.Request.Url);
            Assert.AreEqual(TitleStorageType.GlobalStorage, titleStorageQuota.StorageType);
            VerifyTitleStorageQuotaResult(titleStorageQuota, responseJson);
        }

        [TestMethod]
        public async Task TestGetQuotaAsyncWithTrustedPlatformStorage()
        {
            // Arrange
            var urlExpected = @"https://titlestorage.xboxlive.com/trustedplatform/users/xuid(2814662072777140)/scids/00000000-0000-0000-0000-0000694f5acb";
            MockXboxLiveData.MockRequestData mockRequestData = MockXboxLiveData.MockResponses["TrustedPlatformGetQuota"];
            var responseJson = JObject.Parse(mockRequestData.Response.ResponseBodyString);

            // Act
            var titleStorageQuota = await this.titleStorageService.GetQuotaAsync(user, TitleStorageType.TrustedPlatform);

            // Assert
            Assert.AreEqual("GET", mockRequestData.Request.Method);
            Assert.AreEqual(urlExpected, mockRequestData.Request.Url);
            Assert.AreEqual(TitleStorageType.TrustedPlatform, titleStorageQuota.StorageType);
            VerifyTitleStorageQuotaResult(titleStorageQuota, responseJson);
        }

        [TestMethod]
        public async Task TestGetQuotaAsyncWithUniversalStorage()
        {
            // Arrange
            var urlExpected = @"https://titlestorage.xboxlive.com/universalplatform/users/xuid(2814662072777140)/scids/00000000-0000-0000-0000-0000694f5acb";
            MockXboxLiveData.MockRequestData mockRequestData = MockXboxLiveData.MockResponses["UniversalPlatformGetQuota"];
            var responseJson = JObject.Parse(mockRequestData.Response.ResponseBodyString);

            // Act
            var titleStorageQuota = await this.titleStorageService.GetQuotaAsync(user, TitleStorageType.UniversalPlatform);

            // Assert
            Assert.AreEqual("GET", mockRequestData.Request.Method);
            Assert.AreEqual(urlExpected, mockRequestData.Request.Url);
            Assert.AreEqual(TitleStorageType.UniversalPlatform, titleStorageQuota.StorageType);
            VerifyTitleStorageQuotaResult(titleStorageQuota, responseJson);
        }

        [TestMethod]
        public async Task TestUploadBlobUsingUniversal()
        {
            // Arrange
            var urlExpected = @"https://titlestorage.xboxlive.com/universalplatform/users/xuid(2814662072777140)/scids/00000000-0000-0000-0000-0000694f5acb/data/queststatus/quest01.json,json";
            
            MockXboxLiveData.MockRequestData uploadRequestData = MockXboxLiveData.MockResponses["UniversalPlatformUploadBlob"];
            var titleBlobMetadata = new TitleStorageBlobMetadata(TitleStorageType.UniversalPlatform, @"queststatus/quest01.json", TitleStorageBlobType.Json);
                
            var uploadedQuest = new Quest
            {
                QuestName = "Quest 01",
                CharactersInvolved = new List<string>() { "Character 01", "Character 02", "Character 03" }
            };

            var testQuestJson = JsonSerialization.ToJson(uploadedQuest);
            var blobBuffer = Encoding.ASCII.GetBytes(testQuestJson).ToList();
            var blobQueryProperties = new BlobQueryProperties();

            // Act
            await this.titleStorageService.UploadBlobAsync(user, titleBlobMetadata, blobBuffer, blobQueryProperties );

            // Assert
            Assert.AreEqual("PUT", uploadRequestData.Request.Method);
            Assert.AreEqual(urlExpected, uploadRequestData.Request.Url);
        }

        [TestMethod]
        public async Task TestDownloadBlobUsingUniversal()
        {
            // Arrange
            var urlExpected = @"https://titlestorage.xboxlive.com/universalplatform/users/xuid(2814662072777140)/scids/00000000-0000-0000-0000-0000694f5acb/data/queststatus/quest01.json,json";

            MockXboxLiveData.MockRequestData mockRequestData = MockXboxLiveData.MockResponses["UniversalPlatformDownloadBlob"];
            var responseJson = JObject.Parse(mockRequestData.Response.ResponseBodyString);
            var titleBlobMetadata = new TitleStorageBlobMetadata(TitleStorageType.UniversalPlatform, @"queststatus/quest01.json", TitleStorageBlobType.Json);
            var blobQueryProperties = new BlobQueryProperties();

            // Act
            var downloadBlobResult= await this.titleStorageService.DownloadBlobAsync(user, titleBlobMetadata, blobQueryProperties);
            var downloadBlobString = Encoding.Default.GetString(downloadBlobResult.BlobBuffer.ToArray());
            var downloadedQuest = JsonSerialization.FromJson<Quest>(downloadBlobString);

            // Assert
            Assert.AreEqual("GET", mockRequestData.Request.Method);
            Assert.AreEqual(urlExpected, mockRequestData.Request.Url);
            VerifyDownloadQuest(downloadedQuest, responseJson);
        }

        [TestMethod]
        public async Task TestDeleteBlobUsingUniversal()
        {
            // Arrange
            var urlExpected = @"https://titlestorage.xboxlive.com/universalplatform/users/xuid(2814662072777140)/scids/00000000-0000-0000-0000-0000694f5acb/data/queststatus/quest01.json,json";

            MockXboxLiveData.MockRequestData mockRequestData = MockXboxLiveData.MockResponses["UniversalPlatformDeleteBlob"];
            var titleBlobMetadata = new TitleStorageBlobMetadata(TitleStorageType.UniversalPlatform, @"queststatus/quest01.json", TitleStorageBlobType.Json);
            var blobQueryProperties = new BlobQueryProperties();

            // Act
            await this.titleStorageService.DeleteBlobAsync(user, titleBlobMetadata, blobQueryProperties);

            // Assert
            Assert.AreEqual("DELETE", mockRequestData.Request.Method);
            Assert.AreEqual(urlExpected, mockRequestData.Request.Url);
        }

        [TestMethod]
        public async Task TestUploadBlobUsingUniversalUsingBinaryAndContentLength()
        {
            // Arrange
            var url1Expected = @"https://titlestorage.xboxlive.com/universalplatform/users/xuid(2814662072777140)/scids/00000000-0000-0000-0000-0000694f5acb/data/file.bin,binary?finalBlock=True";
            MockXboxLiveData.MockRequestData uploadRequestData = MockXboxLiveData.MockResponses["UniversalPlatformUploadBinary01"];
            var titleBlobMetadata = new TitleStorageBlobMetadata(TitleStorageType.UniversalPlatform, @"file.bin", TitleStorageBlobType.Binary);
            var longText = "< p >Lorem ipsum dolor sit amet, netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante.Donec eu libero sit amet quam egestas semper. Aenean";
            var blobBuffer = Encoding.ASCII.GetBytes(longText).ToList();
            var blobQueryProperties = new BlobQueryProperties();

            // Act
            await this.titleStorageService.UploadBlobAsync(user, titleBlobMetadata, blobBuffer, blobQueryProperties);

            // Assert
            Assert.AreEqual("PUT", uploadRequestData.Request.Method);
            Assert.AreEqual(url1Expected, uploadRequestData.Request.Url);
        }

        [TestMethod]
        public void GetUploadUrlUsingContinuationToken()
        {
            // Arrange
            var continuationToken = "40b2d28a-da60-4c80-b772-3208a1512bd2-1";
            var titleBlobMetadata = new TitleStorageBlobMetadata(TitleStorageType.UniversalPlatform, @"file.bin", TitleStorageBlobType.Binary);

            // Act
            var urlSubPath = 
                this.titleStorageService.GetTitleStorageBlobMetadataUploadSubpath(user, titleBlobMetadata, continuationToken, false);

            // Assert
            Assert.AreEqual(
                @"universalplatform/users/xuid(2814662072777140)/scids/00000000-0000-0000-0000-0000694f5acb/data/file.bin,binary?continuationToken=40b2d28a-da60-4c80-b772-3208a1512bd2-1&finalBlock=False", 
                urlSubPath);
        }

        private static void VerifyTitleStorageQuotaResult(TitleStorageQuota quotaToVerify, JObject resultToVerify)
        {
            Assert.AreNotEqual(quotaToVerify, null, "TitleStorageQuota was null");

            JObject quotaInfoJson = JObject.Parse(resultToVerify["quotaInfo"].ToString());
            Assert.AreEqual(quotaToVerify.QuotaBytes, (ulong)quotaInfoJson["quotaBytes"]);
            Assert.AreEqual(quotaToVerify.UsedBytes, (ulong)quotaInfoJson["usedBytes"]);
        }

        private static void VerifyDownloadQuest(Quest resultQuest, JObject resultToVerify)
        {
            Assert.AreEqual(resultQuest.QuestName, resultToVerify["QuestName"].ToString());
            var charactersInvolvedExpected = (JArray) resultToVerify["CharactersInvolved"];
            Assert.AreEqual(resultQuest.CharactersInvolved.Count, charactersInvolvedExpected.Count);
            foreach (var character in charactersInvolvedExpected)
            {
                Assert.IsTrue(resultQuest.CharactersInvolved.Contains(character.Value<string>()));
            }
        }
    }
   
    public class Quest
    {
        public string QuestName { get; set; }

        public List<string> CharactersInvolved { get; set; }
    }
}
