using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VTravel.CustomerWeb.Models;


namespace VTravel.CustomerWeb.Controllers
{
    public class PropertyController : Controller
    {
        // GET: PropertyController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PropertyController/Details/5
    
        public IActionResult Details(string id)
        {
            PropertyViewModel propertyViewModel = new PropertyViewModel();
            try
            {

                var idArray = id.Split('-');
                var encodedId = idArray[idArray.Length - 1];
                var decodedId = General.DecodeString(encodedId);
                       
            
                propertyViewModel.property = new Property();
                //get top 10 property list grouped by tags in each group for home displayabl tags

                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SET SESSION group_concat_max_len = 100000;
                                     SELECT 
                                      JSON_OBJECT(
                                    'id',p.id,'title',p.title,'perma_title',p.perma_title,'reserveAllowed',p.reserve_allowed,'reserveAlert',p.reserve_alert,'thumbnail',p.thumbnail,'longDescription',p.long_description
                                    ,'metaTitle',p.meta_title,'metaKeywords',p.meta_keywords,'metaDescription',p.meta_description,'bookingUrl',p.booking_url,'sellOnline',p.sell_online                                        
                                    ,'propertyTypeName',pt.type_name,'city',c.city_name,'state',s.state_name,'country',cn.country_name
                                    ,'maxOccupancy',p.max_occupancy,'roomCount',p.room_count,'bathroomCount',p.bathroom_count
                                    ,'latitude',p.latitude,'longitude',p.longitude
                                    ,'priceList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',pr.id,'priceName',pr.price_name,'mrp',pr.mrp,'price',pr.price)),
                                    ']')
                             AS JSON) from property_price pr where pr.property_id = p.id ORDER BY pr.sort_order)
                                   
                            ,'imageList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',im.id,'url',im.url,'image_alt',im.image_alt,'sortOrder',im.sort_order)),
                                    ']')
                             AS JSON) from property_image im where im.property_id = p.id ORDER BY im.sort_order) 

                            ,'attributeList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',pa.id,'longDescription',pa.long_description,'attributeName',at.attribute_name)),
                                    ']')
                             AS JSON) from property_attribute pa INNER JOIN attribute at ON pa.attribute_id=at.id where pa.property_id = p.id ORDER BY at.sort_order)
 
                           ,'amenityList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',am.id,'amenityName',am.amenity_name,'image1',am.image1)),
                                    ']')
                             AS JSON) from property_amenity pam INNER JOIN amenity am ON pam.amenity_id=am.id AND am.is_active='Y' where pam.property_id = p.id ORDER BY am.sort_order))
                             
                          
                             from property p 
                             INNER JOIN property_type pt ON p.property_type_id=pt.id
                             INNER JOIN city c ON p.city=c.city_code 
                             INNER JOIN state s ON p.state=s.state_code 
                             INNER JOIN country cn ON p.country=cn.country_code  
                             where p.id={0} AND  p.is_active='Y' AND p.property_status='ACTIVE'", Convert.ToInt32(decodedId)
                                );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {

                        propertyViewModel.property = JsonConvert.DeserializeObject<Property>(r[0].ToString());

                    }

                    //sorting
                    propertyViewModel.property.imageList = propertyViewModel.property.imageList.OrderBy(im => im.sortOrder).ToArray<PropertyImage>();



                    ViewData["Title"] = propertyViewModel.property.metaTitle;
                    ViewData["Keywords"] = propertyViewModel.property.metaKeywords;
                    ViewData["Description"] = propertyViewModel.property.metaDescription;
                    ViewData["Thumbnail"] = propertyViewModel.property.thumbnail;
                    ViewData["CanonicalUrl"] =General.GetUrlSlug(propertyViewModel.property.perma_title)+"-"+ encodedId;
                    
                    //get terms

                    query = string.Format(@"SELECT content FROM page WHERE url_slug='terms-and-conditions'
                                  AND is_active='Y'AND page_status='ACTIVE'"
                                    );

                    ds = sqlHelper.GetDatasetByMySql(query);


                    foreach (DataRow r in ds.Tables[0].Rows)
                    {

                        propertyViewModel.terms = r["content"].ToString();

                    }

                    //promo properties

                    propertyViewModel.promoPropertyList = new List<Property>();
                    //get top 10 property list grouped by tags in each group for home displayabl tags

                    var condition = string.Format(" p.id IN(SELECT property_id FROM property_tag WHERE tag_id={0})", 10);

                    query = string.Format(@"SET SESSION group_concat_max_len = 100000;
                                     SELECT 
                                      JSON_OBJECT(
                                    'id',p.id,'title',p.title,'perma_title',p.perma_title,'thumbnail',p.thumbnail  
                                     ,'sortOrder',p.sort_order
                                    ,'city',c.city_name,'state',s.state_name,'country',cn.country_name
                                    ,'priceList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',pr.id,'priceName',pr.price_name,'mrp',pr.mrp,'price',pr.price)),
                                    ']')
                             AS JSON) from property_price pr where pr.property_id = p.id ORDER BY pr.sort_order))
                                
                          
                             from property p 
                             INNER JOIN city c ON p.city=c.city_code 
                             INNER JOIN state s ON p.state=s.state_code 
                             INNER JOIN country cn ON p.country=cn.country_code  
                             where p.is_active='Y' AND p.property_status='ACTIVE' AND {0} AND p.id !={1}", condition, decodedId
                                    );

                    ds = sqlHelper.GetDatasetByMySql(query);


                    foreach (DataRow r in ds.Tables[0].Rows)
                    {

                        propertyViewModel.promoPropertyList.Add(JsonConvert.DeserializeObject<Property>(r[0].ToString()));

                    }
                }
                else
                {
                    return Redirect("Home/Error");
                }
              


            }
            catch (Exception ex)
            {
                General.LogException(ex);

                return Redirect("Home/Error");
            }
            return View(propertyViewModel);
        }

        public IActionResult List(string id1, string id2)
        {
            PropertyViewModel propertyViewModel = new PropertyViewModel();
            try
            {
                MySqlHelper sqlHelper = new MySqlHelper();
                DataSet ds;
                var query = "";

                var title = "";
                var metaTitle = "";
                var metaKeywords = "";
                var description = "";
                var short_desc = "";
                var metaDescription = "";
                var condition = "";
                var long_desc = "";

                var thumbnail = "";

                var centre_latitude = "9.931233";
                var centre_longitude = "76.267303";

                if (id1 == "search")
                {
                    var id2Array = id2.Split('-');
                    var latitude = id2Array[0];
                    var longitude = id2Array[1];
                    var address = "";
                    if (id2Array.Length > 2)
                    {
                        address = id2Array[2];
                    }

                    centre_latitude = latitude;
                    centre_longitude = longitude;

                    condition = string.Format(@" (6371 * acos(
                            cos(radians(p.latitude))
                          * cos(radians({0}))
                          * cos(radians({1}) - radians(p.longitude))
                          + sin(radians(p.latitude))
                          * sin(radians({0}))
                            ) ) <=p.display_radius AND p.hide_property = '0'", latitude, longitude);

                    title = address;
                    description = "Explore the best holiday homes in " + address;
                    metaKeywords = "Explore the best holiday homes in " + address;
                    metaTitle = "Explore the best holiday homes in " + address;
                    metaDescription = "Explore the best holiday homes in " + address;
                    propertyViewModel.banner_url = "";
                }
                else if (id1 == "destination")
                {


                    var id2Array = id2.Split('-');
                    var encodedId2 = id2Array[id2Array.Length - 1];
                    var decodedId2 = General.DecodeString(encodedId2);

                    condition = "p.hide_property = '0' AND p.destination_id=" + decodedId2;

                    //get destination details
                    query = string.Format(@"SELECT id,image1, title,description,thumbnail,meta_title,meta_keywords,meta_description
                                            , IFNULL(short_desc, '') short_desc , IFNULL(long_desc, '') long_desc    
                                            FROM destination WHERE  is_active='Y' AND id={0}",
                                           decodedId2);


                    ds = sqlHelper.GetDatasetByMySql(query);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {

                                foreach (DataRow r in ds.Tables[0].Rows)
                                {

                                    var dest = (
                                        new Destination
                                        {
                                            id = Convert.ToInt32(r["id"].ToString()),
                                            thumbnail = r["thumbnail"].ToString(),
                                            title = r["title"].ToString(),
                                            description = r["description"].ToString(),
                                            short_desc = r["short_desc"].ToString(),
                                            banner_url = r["image1"].ToString(),
                                            long_desc = r["long_desc"].ToString(),

                                        });

                                    title = dest.title;
                                    description = dest.description;
                                    short_desc = dest.short_desc;
                                    metaKeywords = r["meta_keywords"].ToString();
                                    metaTitle = r["meta_title"].ToString();
                                    metaDescription = r["meta_description"].ToString();
                                    thumbnail = dest.thumbnail;
                                    propertyViewModel.banner_url = dest.banner_url;
                                    long_desc = dest.long_desc;

                                    ViewData["CanonicalUrl"] = "destination/" + General.GetUrlSlug(dest.title) + "-" + encodedId2;
                                }


                            }

                        }

                    }
                }
                else if (id1 == "category")
                {
                    var id2Array = id2.Split('-');
                    var encodedId2 = id2Array[id2Array.Length - 1];
                    var decodedId2 = General.DecodeString(encodedId2);

                    condition = string.Format(" p.id IN(SELECT property_id FROM property_tag WHERE tag_id={0}) AND p.hide_property = '1'", decodedId2);

                    //get tag details
                    query = string.Format(@"SELECT id,image1, tag_name,meta_title,meta_keywords,meta_description   
                                           FROM tag WHERE  is_active='Y' AND id={0}",
                                          decodedId2);


                    ds = sqlHelper.GetDatasetByMySql(query);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {

                                foreach (DataRow r in ds.Tables[0].Rows)
                                {

                                    var tag = (
                                        new Tag
                                        {
                                            id = Convert.ToInt32(r["id"].ToString()),
                                            tagName = r["tag_name"].ToString(),
                                            banner_url = r["image1"].ToString(),


                                        });

                                    title = r["tag_name"].ToString();
                                    description = r["meta_description"].ToString();
                                    metaTitle = r["meta_title"].ToString();
                                    metaDescription = r["meta_description"].ToString();
                                    metaKeywords = r["meta_keywords"].ToString();
                                    propertyViewModel.banner_url = tag.banner_url;

                                    ViewData["CanonicalUrl"] = "category/" + General.GetUrlSlug(tag.tagName) + "-" + encodedId2;

                                }


                            }

                        }

                    }


                }



                propertyViewModel.propertyList = new List<Property>();
                //get top 10 property list grouped by tags in each group for home displayabl tags



                query = string.Format(@"SET SESSION group_concat_max_len = 100000;
                                     SELECT 
                                      JSON_OBJECT(
                                    'id',p.id,'title',p.title,'perma_title',p.perma_title,'thumbnail',p.thumbnail  
                                     ,'sortOrder',p.sort_order
                                    ,'city',c.city_name,'state',s.state_name,'country',cn.country_name

                                    ,'distance', SQRT(
                                POW(69.1 * (p.latitude - {0}), 2) +
                                POW(69.1 * ({1} - p.longitude) * COS(p.latitude / 57.3), 2))

                                    ,'priceList',(SELECT CAST(CONCAT('[',
                                    GROUP_CONCAT(
                                      JSON_OBJECT(
                                        'id',pr.id,'priceName',pr.price_name,'mrp',pr.mrp,'price',pr.price)),
                                    ']')
                             AS JSON) from property_price pr where pr.property_id = p.id ORDER BY pr.sort_order))
                                
                          
                             from property p 
                             INNER JOIN city c ON p.city=c.city_code 
                             INNER JOIN state s ON p.state=s.state_code 
                             INNER JOIN country cn ON p.country=cn.country_code  
                             where p.is_active='Y' AND p.property_status='ACTIVE' AND {2}"
                              , centre_latitude, centre_longitude, condition
                                );

                ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    propertyViewModel.propertyList.Add(JsonConvert.DeserializeObject<Property>(r[0].ToString()));

                }

                //sorting
                if (id1 == "search")
                {
                    propertyViewModel.propertyList = propertyViewModel.propertyList.OrderBy(p => p.distance).ToList<Property>();
                }
                else
                {
                    propertyViewModel.propertyList = propertyViewModel.propertyList.OrderBy(p => p.sortOrder).ToList<Property>();
                }
                
                if (propertyViewModel.propertyList.Count == 0)
                {
                    title = "Sorry, there are no properties available matching your search";
                }
                propertyViewModel.title = title;
                propertyViewModel.short_desc = short_desc;
                propertyViewModel.long_desc = long_desc;





                ViewData["Title"] = metaTitle;
                ViewData["Keywords"] = metaKeywords;
                ViewData["Description"] = metaDescription;
                ViewData["Thumbnail"] = thumbnail;




            }
            catch (Exception ex)
            {
                General.LogException(ex);
                return Redirect("Home/Error");
            }
            return View(propertyViewModel);
        }
    }
}
