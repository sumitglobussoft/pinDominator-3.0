using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PinDominator.Account
{
    public class PCQueue
    {
        //public static Events LogEvent = new Events();

        //public static Events bannedURLEvent = new Events();

        readonly object _locker = new object();

        Thread[] _workers;

        public PCQueue()
        {
            //_workers = new Thread[workerCount];

            //// Create and start a separate thread for each worker
            //for (int i = 0; i < workerCount; i++)
            //    (_workers[i] = new Thread(Consume)).Start();
        }

        public void Shutdown(bool waitForWorkers)
        {
            // Enqueue one null item per worker to make each exit.
            foreach (Thread worker in _workers)
                EnqueueItem(null);

            // Wait for workers to finish
            if (waitForWorkers)
                foreach (Thread worker in _workers)
                    worker.Join();
        }

        public void StopConsumerThreads()
        {
            foreach (Thread worker in _workers)
            {
                try
                {
                    worker.Abort();
                }
                catch (Exception)
                {
                }
            }

        }

        public void EnqueueItem(string[] item)
        {
            if (PInterestAccountManager_Producer._SeedUrlQueue.Count >= 100000)
            {
                Thread.Sleep(900000);
            }

            lock (_locker)
            {
                //Thread.Sleep(2000);

                PInterestAccountManager_Producer._SeedUrlQueue.Enqueue(item);           // We must pulse because we're
                Monitor.Pulse(_locker);         // changing a blocking condition.

            }
        }



        public void Consume()
        {
            while (true)                        // Keep consuming until
            {                                   // told otherwise.

                try
                {
                    Random random = new Random();
                    string[] item;

                    string urltobecrawled = string.Empty;
                    string type = string.Empty;
                    string proxy = string.Empty;


                    if (PInterestAccountManager_Producer.UseProxy)
                    {
                        #region Using Proxies

                        #region Crawl in Yahoo
                        lock (PInterestAccountManager_Producer._locker)
                        {
                            while (PInterestAccountManager_Producer._SeedUrlQueue.Count == 0)
                            {
                                Monitor.Wait(PInterestAccountManager_Producer._locker);
                            }
                            try
                            {
                                item = PInterestAccountManager_Producer._SeedUrlQueue.Dequeue();

                                urltobecrawled = item[0];
                                type = item[1];
                                proxy = PInterestAccountManager_Producer.listWorkingProxiesYahoo[random.Next(0, PInterestAccountManager_Producer.listWorkingProxiesYahoo.Count - 1)];

                                //Console.WriteLine("Dequed " + urltobecrawled);
                                //Log("Dequed " + urltobecrawled);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        //if (item == null) return;         // This signals our exit.
                        //item();                           // Execute item.

                        Console.WriteLine("Dequed " + urltobecrawled);
                        //Log("Dequed " + urltobecrawled);

                        if (!string.IsNullOrEmpty(urltobecrawled))
                        {
                            ///if its yahoo url then use yahoo crawler
                            if (urltobecrawled.Contains("yahoo.com"))
                            {
                                CrawlYahooUsingProxy(urltobecrawled, proxy, type);
                            }
                        }
                        #endregion

                        #region Crawl in Google
                        lock (PInterestAccountManager_Producer._locker)
                        {
                            while (PInterestAccountManager_Producer._SeedUrlQueue.Count == 0)
                            {
                                Monitor.Wait(PInterestAccountManager_Producer._locker);
                            }
                            try
                            {
                                item = PInterestAccountManager_Producer._SeedUrlQueue.Dequeue();

                                urltobecrawled = item[0];
                                type = item[1];
                                proxy = PInterestAccountManager_Producer.listWorkingProxiesGoogle[random.Next(0, PInterestAccountManager_Producer.listWorkingProxiesGoogle.Count - 1)];

                                //Console.WriteLine("Dequed " + urltobecrawled);
                                //Log("Dequed " + urltobecrawled);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        //if (item == null) return;         // This signals our exit.
                        //item();                           // Execute item.

                        Console.WriteLine("Dequed " + urltobecrawled);
                        //Log("Dequed " + urltobecrawled);

                        ///Using proxies...
                        if (!string.IsNullOrEmpty(urltobecrawled))
                        {
                            ///if its a google url then use google crawler
                            if (urltobecrawled.Contains("google.com"))
                            {
                                CrawlGoogleUsingProxy(urltobecrawled, proxy, type);
                            }
                        }
                        #endregion

                        #endregion
                    }

                    else
                    {
                        #region Without Proxies...
                        lock (PInterestAccountManager_Producer._locker)
                        {
                            while (PInterestAccountManager_Producer._SeedUrlQueue.Count == 0)
                            {
                                Monitor.Wait(PInterestAccountManager_Producer._locker);
                            }
                            try
                            {
                                item = PInterestAccountManager_Producer._SeedUrlQueue.Dequeue();

                                urltobecrawled = item[0];
                                type = item[1];

                                //Console.WriteLine("Dequed " + urltobecrawled);
                                //Log("Dequed " + urltobecrawled);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        //if (item == null) return;         // This signals our exit.
                        //item();                           // Execute item.

                        Console.WriteLine("Dequed " + urltobecrawled);
                        //Log("Dequed " + urltobecrawled);

                        if (!string.IsNullOrEmpty(urltobecrawled))
                        {
                            //if its a google url then use google crawler
                            if (urltobecrawled.Contains("google.com"))
                            {
                                CrawlInGoogle(urltobecrawled, type);
                            }
                            //if its yahoo url then use yahoo crawler
                            if (urltobecrawled.Contains("yahoo.com"))
                            {
                                CrawlInYahoo(urltobecrawled, type);
                            }
                        }
                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    //Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
                }

            }
        }



        private void CrawlInGoogle(string searchQuery, string type)
        {
            //try
            //{
            //    //GoogleCrawler googleCrawler = new GoogleCrawler();
            //    List<string> listBlogURLs = googleCrawler.GetBlogURLs(searchQuery);

            //    if (listBlogURLs == null || listBlogURLs.Count == 0)
            //    {
            //        PInterestAccountManager_Producer._SeedUrlQueue.Enqueue(new string[] { searchQuery, type });
            //        IncrementPostedURL("Google");
            //        return;
            //    }

            //    ///Add URLs to the corresponding BlogType Crawled list...
            //    if (type == "WP")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                //DataBaseHandler.InsertQuery(InsertQuery, "tablename");
            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsWP.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsWP.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerWP)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerWP);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "B2E")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                //DataBaseHandler.InsertQuery(InsertQuery, "tablename");
            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsB2E.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsB2E.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                //lock (PCManagerPost._lockerB2E)
            //                //{
            //                //    Monitor.Pulse(PCManagerPost._lockerB2E);
            //                //}
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "MT")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                //DataBaseHandler.InsertQuery(InsertQuery, "tablename");
            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsMT.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsMT.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                //lock (PCManagerPost._lockerMT)
            //                //{
            //                //    Monitor.Pulse(PCManagerPost._lockerMT);
            //                //}
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

        private void CrawlInYahoo(string searchQuery, string type)
        {
            //try
            //{
            //    YahooCrawler yahooCrawler = new YahooCrawler();
            //    List<string> listBlogURLs = yahooCrawler.GetBlogURLs(searchQuery);

            //    if (listBlogURLs == null || listBlogURLs.Count == 0)
            //    {
            //        PInterestAccountManager_Producer._SeedUrlQueue.Enqueue(new string[] { searchQuery, type });
            //        IncrementPostedURL("Yahoo");
            //        return;
            //    }

            //    ///Filter unwanted content
            //    ///when crawling from Yahoo, first 3 URLs are of no use
            //    if (listBlogURLs.Count > 3)
            //    {
            //        listBlogURLs.RemoveRange(0, 3);
            //    }

            //    ///Add URLs to the corresponding BlogType Crawled list...
            //    if (type == "WP")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");
            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsWP.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsWP.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerWP)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerWP);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "B2E")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");
            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsB2E.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsB2E.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerB2E)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerB2E);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "MT")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");
            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsMT.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsMT.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerMT)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerMT);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

        private void CrawlGoogleUsingProxy(string searchQuery, string proxy, string type)
        {
            //try
            //{
            //    GoogleCrawler googleCrawler = new GoogleCrawler();

            //    List<string> listBlogURLs = null;

            //    if (proxy.Split(':').Length < 4)        //Public proxies
            //    {
            //        listBlogURLs = googleCrawler.GetBlogURLsProxy(searchQuery, proxy.Split(':')[0], proxy.Split(':')[1], "", "");
            //    }

            //    else         //Private proxies
            //    {
            //        listBlogURLs = googleCrawler.GetBlogURLsProxy(searchQuery, proxy.Split(':')[0], proxy.Split(':')[1], proxy.Split(':')[2], proxy.Split(':')[3]);
            //    }

            //    ///If no list returns, Enqueue back the Seed URL, so that it's not wasted
            //    if (listBlogURLs == null || listBlogURLs.Count == 0)
            //    {
            //        PInterestAccountManager_Producer._SeedUrlQueue.Enqueue(new string[] { searchQuery, type });

            //        //Remove faulty proxy from list
            //        PInterestAccountManager_Producer.listWorkingProxiesYahoo.Remove(proxy);

            //        IncrementPostedURL("Google");

            //        return;
            //    }

            //    ///Add URLs to the corresponding BlogType Crawled list...
            //    if (type == "WP")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");

            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsWP.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsWP.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerWP)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerWP);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "B2E")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");

            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsB2E.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsB2E.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerB2E)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerB2E);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "MT")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");

            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsMT.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsMT.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerMT)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerMT);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

        private void CrawlYahooUsingProxy(string searchQuery, string proxy, string type)
        {
            //try
            //{
            //    YahooCrawler yahooCrawler = new YahooCrawler();

            //    List<string> listBlogURLs = null;

            //    if (proxy.Split(':').Length < 4)         //Public proxies
            //    {
            //        listBlogURLs = yahooCrawler.GetBlogURLsProxy(searchQuery, proxy.Split(':')[0], proxy.Split(':')[1], "", "");
            //    }

            //    else         //Private proxies
            //    {
            //        listBlogURLs = yahooCrawler.GetBlogURLsProxy(searchQuery, proxy.Split(':')[0], proxy.Split(':')[1], proxy.Split(':')[2], proxy.Split(':')[3]);
            //    }

            //    ///If no list returns, Enqueue back the Seed URL, so that it's not wasted
            //    if (listBlogURLs == null || listBlogURLs.Count == 0)
            //    {
            //        PInterestAccountManager_Producer._SeedUrlQueue.Enqueue(new string[] { searchQuery, type });

            //        //Remove faulty proxy from list
            //        PInterestAccountManager_Producer.listWorkingProxiesYahoo.Remove(proxy);

            //        IncrementPostedURL("Yahoo");

            //        return;
            //    }

            //    ///Filter unwanted content
            //    ///when crawling from Yahoo, first 3 URLs are of no use
            //    if (listBlogURLs.Count > 3)
            //    {
            //        listBlogURLs.RemoveRange(0, 3);
            //    }

            //    ///Add URLs to the corresponding BlogType Crawled list...
            //    if (type == "WP")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");

            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsWP.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsWP.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerWP)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerWP);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "B2E")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");

            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsMT.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsB2E.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerB2E)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerB2E);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }
            //    else if (type == "MT")
            //    {
            //        foreach (string BlogURL in listBlogURLs)
            //        {
            //            try
            //            {
            //                string InsertQuery = "Insert into tb_CrawledURLs (URL, IsSearched) values('" + BlogURL + "','" + "true" + "')";
            //                DataBaseHandler.InsertQuery(InsertQuery, "tablename");

            //                //PInterestAccountManager_Producer.FinalCrawledURLs.Add(BlogURL, BlogURL);
            //                //PInterestAccountManager_Producer.FinalCrawledURLsMT.Add(BlogURL, BlogURL);
            //                PInterestAccountManager_Producer.QuePostURLsMT.Enqueue(BlogURL);

            //                ///Pulse locker in PCManagerPost so that posting Monitor.Wait is awakened and posting continues
            //                ///as soon as any item is enqued
            //                lock (PCManagerPost._lockerMT)
            //                {
            //                    Monitor.Pulse(PCManagerPost._lockerMT);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.StackTrace);
            //                Globussoft.GlobusFileHelper.AppendStringToTextfileNewLineWithCarat(ex.StackTrace, GlobalPaths.errorLogFilePath);
            //            }
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

        public void IncrementPostedURL(string number)
        {
            //EventsArgs eventsArgs = new EventsArgs(number);
            //bannedURLEvent.IncreaseCounter(eventsArgs);
        }

        public void Log(string log)
        {
            //EventsArgs eventsArgs = new EventsArgs(log);
            //LogEvent.LogText(eventsArgs);
        }





    }
}
