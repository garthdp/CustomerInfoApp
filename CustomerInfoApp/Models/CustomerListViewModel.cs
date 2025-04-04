namespace CustomerInfoApp.Models
{
    public class CustomerListViewModel
    {
        public CustomerListViewModel(IEnumerable<CustomerInfo> customers, int currentPage, int totalPages, string sortDirection, string searchName, string searchNum)
        {
            Customers = customers;
            CurrentPage = currentPage;
            TotalPages = totalPages;
            SortDirection = sortDirection;
            SearchName = searchName;
            SearchNum = searchNum;
        }

        public IEnumerable<CustomerInfo> Customers { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortDirection { get; set; }
        public string SearchName { get; set; }
        public string SearchNum { get; set; }
    }
}
