import { createBrowserRouter } from "react-router-dom";

import RootLayout from "./pages/common/RootLayout";
import Index from "./pages/common/Index";
import Shop from "./pages/shop/Shop";
import DishItem from "./pages/shop/DishItem";
import RouteErrorBoundary from "./pages/common/RouteErrorBoundary";
import AdminIndex from "./pages/admin/AdminIndex";
import EditDish from "./pages/admin/EditDish";
import AddDish from "./pages/admin/AddDish";

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
        ]
      }
    ],
  },
]);

export default router;
