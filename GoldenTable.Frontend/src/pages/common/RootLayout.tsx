import type { ReactNode } from "react";
import { Outlet } from "react-router-dom";
import MainHeader from "../../components/MainHeader";

function RootLayout(): ReactNode {
  return (
    <>
      <MainHeader />
      <main>
        <Outlet />
      </main>
    </>
  );
}

export default RootLayout;
