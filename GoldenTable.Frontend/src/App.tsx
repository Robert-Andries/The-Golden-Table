import { createBrowserRouter, RouterProvider } from "react-router-dom";

import RootLayout from "./pages/RootLayout";
import ErrorPage from "./pages/Error";
import Index from "./pages/Index";
import Shop from "./pages/Shop";

const router = createBrowserRouter([
  {
    path: "/",
    element: <RootLayout />,
    errorElement: <ErrorPage />,
    children: [
      { index: true, element: <Index /> },
      { path: "shop", element: <Shop /> },
    ],
  },
]);

const App: React.FC = () => <RouterProvider router={router} />;

export default App;
