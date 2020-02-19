# ImageHelper base on Github

Tutorial
----

一套 library 可以讓你無 server 配合 Github 做出一套 Image Service ，簡單說就可以把 github 當圖床用?，因為這是 library 版本 ，如果你是想要直接使用 Server 版本，你可以到這裡 https://github.com/donma/N2ImageAgent.AzureBlob2020 ，使用 Azure 版本，除非我找時間來寫 sever 版本 base on Github 。 這套 library 製作出來的 repo 會長這樣 https://github.com/gitozhack/imgstock1 ，如果有幫助就幫我 star 一下了，感恩。

Happy Coding :)

Document
----

#### Init Github ContainerInfo
```C#

            //your_github_token get from : https://github.com/settings/tokens
            var containerInfo = new N2ImageAgentGithub.ContainerInfo("your_github_token", "your_username", "yout_reponame");

            var agent = new N2ImageAgentGithub.Agent(containerInfo);

```

#### Upload Image From Local File.
```C#
           Console.WriteLine("Upload Image To Source");
           agent.UpoloadImageToSource("test1", AppDomain.CurrentDomain.BaseDirectory + "sample1.jpg", "test upload image source");
     
```

#### Upload Image From File Byte[]

```C#
            Console.WriteLine("Upload Image To Source from byte[]");
            agent.UpoloadImageToSource("test1", File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "sample1.jpg"), "測試上傳:"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

```

#### Get Image Info By Utility and Upload Info

```C#
           Console.WriteLine("Get Image Info And Upload");
           var info = Utility.GetImageInfo(AppDomain.CurrentDomain.BaseDirectory + "sample1.jpg", "imageid", "taginfo");
           Console.WriteLine(JsonConvert.SerializeObject(info));
           agent.UpoloadImageInfo(info, "測試上傳:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

```

#### Delete Image Info And Source By Id

```C#
            Console.WriteLine("Delete Source and Info");
            agent.DeleteSource("imageid");
            agent.DeleteImageInfo("imageid");
```

#### Delete All Thumb By Id

```C#
            Console.WriteLine("Delete All Thumb");
            agent.DeleteAllImageById("imageid",1000);

```

#### Is Image Source Exist

```C#
            Console.WriteLine("Check image in Source");
            agent.IsSourceExisted("imageid");

```

#### Is Image Info Exist

```C#
  
            Console.WriteLine("Check image info is existed");
            agent.IsImageInfoExisted("imageid");

```

#### Is Image Thumb Exist

```C#
           Console.WriteLine("Check image info is existed");
           agent.IsImageThumbExisted("imageid");

```

#### Get Image source or thumb url from Github

```C#
            //source 
            var source_url = agent.GetImageThumbFromSource("imageid", 0, 0);
            Console.WriteLine(source_url);
            //thumb1 : get width 100 and height depend on width.
            var thumb1 = agent.GetImageThumbFromSource("imageid", 100, 0);
            Console.WriteLine(thumb1);
            //thumb2 : get height 200 and width depend on height.
            var thumb2 = agent.GetImageThumbFromSource("imageid", 0, 200);
            Console.WriteLine(thumb2);
            

```


