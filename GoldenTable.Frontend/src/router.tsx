import { createBrowserRouter } from "react-router-dom";

import RootLayout from "./pages/common/RootLayout";
import Index from "./pages/common/Index";
import DishItem from "./pages/shop/DishItem";
import RouteErrorBoundary from "./pages/common/RouteErrorBoundary";
import AdminIndex from "./pages/admin/AdminIndex";
import ManageImages from "./pages/admin/images/ManageImages";
import Shop from "./pages/shop/Shop";
import EditDish from "./pages/admin/dish/EditDish";
import AddDish from "./pages/admin/dish/AddDish";
import Tags from "./pages/admin/tags/Tags";

const router = createBrowserRouter([
  {
    path: "/",
    element: <RootLayout />,
    errorElement: <RouteErrorBoundary />,
    children: [
      { index: true, element: <Index /> },
      {
        path: "shop",
        children: [
          { index:true, element: <Shop />},
          { path: ":id",element: <DishItem />},
        ],
      },
      {
        path: "admin",
        children: [
          {index: true, element: <AdminIndex />},
          {path: "edit/:id", element: <EditDish />},
          {path: "add", element: <AddDish />},
          {path: "images", element: <ManageImages />},
          {path: "tags", element: <Tags />}
        ]
      }
    ],
  },
]);

export default router;
