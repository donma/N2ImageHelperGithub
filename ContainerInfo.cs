using System;

namespace N2ImageAgentGithub
{
    public class ContainerInfo
    {


        /// <summary>
        /// Github Token 
        /// Create from : https://github.com/settings/tokens
        /// </summary>
        public string GithubToken { get; set; }


        /// <summary>
        /// Your Github UserName
        /// </summary>
        public string GithubName { get; set; }


        /// <summary>
        /// Repository Name
        /// </summary>
        public string RepoName { get; set; }

        /// <summary>
        /// Anychars
        /// </summary>
        public string ProductHeaderValue { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="githubToken">Github Token </param>
        /// <param name="githubName">Your Github UserName</param>
        /// <param name="repoName">Repository Name</param>
        /// <param name="productHeadre">Anychars</param>
        public ContainerInfo(string githubToken, string githubName, string repoName,string productHeadre="N2ImageAgent")
        {
            if (string.IsNullOrEmpty(githubToken)) throw new ArgumentNullException("githubToken");
            if (string.IsNullOrEmpty(githubName)) throw new ArgumentNullException("githubName");
            if (string.IsNullOrEmpty(repoName)) throw new ArgumentNullException("repoName");
            if (string.IsNullOrEmpty(productHeadre)) throw new ArgumentNullException("productHeadre");
            GithubToken = githubToken;
            GithubName = githubName;
            RepoName = repoName;
            ProductHeaderValue = productHeadre;
        }


    }
}
