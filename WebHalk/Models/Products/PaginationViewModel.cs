namespace WebHalk.Models.Products
{
    public class PaginationViewModel
    {
        //Кількість усіх записів
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}