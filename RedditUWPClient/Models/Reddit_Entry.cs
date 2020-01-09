using Newtonsoft.Json;
using RedditUWPClient.ExtensionsMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedditUWPClient.Models
{

    public class Reddit_Entry 
    {
        public string kind { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string modhash { get; set; }
        public int dist { get; set; }
        public List<Child> children { get; set; }
        public string after { get; set; }
        public object before { get; set; }
    }

    public class Child
    {
        public string kind { get; set; }
        public Data1 data { get; set; }
    }

    public class Data1 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object approved_at_utc { get; set; }
        public string subreddit { get; set; }
        public string selftext { get; set; }
        public string author_fullname { get; set; }
        public bool saved { get; set; }
        public object mod_reason_title { get; set; }
        public int gilded { get; set; }
        public bool clicked { get; set; }
        public string title { get; set; }
        public object[] link_flair_richtext { get; set; }
        public string subreddit_name_prefixed { get; set; }
        public bool hidden { get; set; }
        public int pwls { get; set; }
        public string link_flair_css_class { get; set; }
        public int downs { get; set; }
        public int thumbnail_height { get; set; }
        public bool hide_score { get; set; }
        public string name { get; set; }
        public bool quarantine { get; set; }
        public string link_flair_text_color { get; set; }
        public object author_flair_background_color { get; set; }
        public string subreddit_type { get; set; }
        public int ups { get; set; }
        public int total_awards_received { get; set; }
        public Media_Embed media_embed { get; set; }
        public int thumbnail_width { get; set; }
        public object author_flair_template_id { get; set; }
        public bool is_original_content { get; set; }
        public object[] user_reports { get; set; }
        public object secure_media { get; set; }
        public bool is_reddit_media_domain { get; set; }
        public bool is_meta { get; set; }
        public object category { get; set; }
        public Secure_Media_Embed secure_media_embed { get; set; }
        public string link_flair_text { get; set; }
        public bool can_mod_post { get; set; }
        public int score { get; set; }
        public object approved_by { get; set; }
        public bool author_premium { get; set; }
        public string thumbnail { get; set; }
        public bool edited { get; set; }
        public object author_flair_css_class { get; set; }
        public object[] steward_reports { get; set; }
        public object[] author_flair_richtext { get; set; }
        public Gildings gildings { get; set; }
        public string post_hint { get; set; }
        public object content_categories { get; set; }
        public bool is_self { get; set; }
        public object mod_note { get; set; }
        public float created { get; set; }
        public string link_flair_type { get; set; }
        public int wls { get; set; }
        public object removed_by_category { get; set; }
        public object banned_by { get; set; }
        public string author_flair_type { get; set; }
        public string domain { get; set; }
        public bool allow_live_comments { get; set; }
        public object selftext_html { get; set; }
        public object likes { get; set; }
        public object suggested_sort { get; set; }
        public object banned_at_utc { get; set; }
        public object view_count { get; set; }
        public bool archived { get; set; }
        public bool no_follow { get; set; }
        public bool is_crosspostable { get; set; }
        public bool pinned { get; set; }
        public bool over_18 { get; set; }
        public Preview preview { get; set; }
        public All_Awardings[] all_awardings { get; set; }
        public string[] awarders { get; set; }
        public bool media_only { get; set; }
        public string link_flair_template_id { get; set; }
        public bool can_gild { get; set; }
        public bool spoiler { get; set; }
        public bool locked { get; set; }
        public object author_flair_text { get; set; }
        public bool visited { get; set; }
        public object removed_by { get; set; }
        public object num_reports { get; set; }
        public object distinguished { get; set; }
        public string subreddit_id { get; set; }
        public object mod_reason_by { get; set; }
        public object removal_reason { get; set; }
        public string link_flair_background_color { get; set; }
        public string id { get; set; }
        public bool is_robot_indexable { get; set; }
        public object report_reasons { get; set; }
        public string author { get; set; }
        public object discussion_type { get; set; }
        public int num_comments { get; set; }
        public bool send_replies { get; set; }
        public string whitelist_status { get; set; }
        public bool contest_mode { get; set; }
        public object[] mod_reports { get; set; }
        public bool author_patreon_flair { get; set; }
        public object author_flair_text_color { get; set; }
        public string permalink { get; set; }
        public string parent_whitelist_status { get; set; }
        public bool stickied { get; set; }
        public string url { get; set; }
        public int subreddit_subscribers { get; set; }
        public float created_utc { get; set; }
        public int num_crossposts { get; set; }
        public object media { get; set; }
        public bool is_video { get; set; }


        [JsonIgnore]
        public string TimeAgo
        {
            get
            {
                DateTime dt = new DateTime().UnixUTCTimeToLocalDateTime(this.created_utc);
                TimeSpan diff = (DateTime.Now - dt);

                if (diff.TotalHours < 1)
                {
                    return Math.Floor(diff.TotalMinutes) + " minutes ago";
                }
                else if (diff.TotalDays < 1)
                {
                    return Math.Floor(diff.TotalHours) + " hours ago";
                }
                else
                {
                    return Math.Floor(diff.TotalDays) + " days ago";
                }


            }
        }

        [JsonIgnore]
        public string Comments
        {
            get
            {
                if (num_comments == 0)
                {
                    return "No comments";
                }
                else if (num_comments == 1)
                {
                    return "One comment";
                }
                else if (num_comments > 1000)
                {
                    return Math.Round((decimal)num_comments / 1000, 1) + "k comments";
                }
                else if (num_comments > 1000000)
                {
                    return Math.Round((decimal)num_comments / 1000000, 1) + "M comments";
                }
                else
                {
                    return num_comments + " comments";
                }


            }
        }

        private bool _Read = false;
        [JsonIgnore]
        public bool Read { get { return _Read; } 
            set { _Read = value;
                NotifyPropertyChanged();
            } }
        
    }

    public class Media_Embed
    {
    }

    public class Secure_Media_Embed
    {
    }

    public class Gildings
    {
        public int gid_1 { get; set; }
        public int gid_2 { get; set; }
        public int gid_3 { get; set; }
    }

    public class Preview
    {
        public List<Image> images { get; set; }
        public bool enabled { get; set; }
    }

    public class Image
    {
        public Source source { get; set; }
        public Resolution[] resolutions { get; set; }
        public Variants variants { get; set; }
        public string id { get; set; }
    }

    public class Source
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Variants
    {
    }

    public class Resolution
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class All_Awardings
    {
        public int count { get; set; }
        public bool is_enabled { get; set; }
        public string subreddit_id { get; set; }
        public string description { get; set; }
        public int? end_date { get; set; }
        public string award_sub_type { get; set; }
        public int coin_reward { get; set; }
        public string icon_url { get; set; }
        public int days_of_premium { get; set; }
        public bool is_new { get; set; }
        public string id { get; set; }
        public int icon_height { get; set; }
        public Resized_Icons[] resized_icons { get; set; }
        public int days_of_drip_extension { get; set; }
        public string award_type { get; set; }
        public int? start_date { get; set; }
        public int coin_price { get; set; }
        public int icon_width { get; set; }
        public int subreddit_coin_reward { get; set; }
        public string name { get; set; }
    }

    public class Resized_Icons
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    

    
}


public class SamplingData
{

    public static ObservableCollection<RedditUWPClient.Models.Child> RedditEntries
    {
        get
        {
            return new ObservableCollection<RedditUWPClient.Models.Child>()
                {
                    new RedditUWPClient.Models.Child { data = new RedditUWPClient.Models.Data1 { title = "Title1", author = "Author1", created_utc = 1578528573, num_comments = 1 } },
                    new RedditUWPClient.Models.Child { data = new RedditUWPClient.Models.Data1 { title = "Title2", author = "Author2", created_utc = 1578528573, num_comments = 2 } },
                    new RedditUWPClient.Models.Child { data = new RedditUWPClient.Models.Data1 { title = "Title3", author = "Author3", created_utc = 1578528573, num_comments = 3 } },
                    new RedditUWPClient.Models.Child { data = new RedditUWPClient.Models.Data1 { title = "Title4", author = "Author4", created_utc = 1578528573, num_comments = 4 } },
                    new RedditUWPClient.Models.Child { data = new RedditUWPClient.Models.Data1 { title = "Title5", author = "Author5", created_utc = 1578528573, num_comments = 5 } },
                };
        }

    }

}
