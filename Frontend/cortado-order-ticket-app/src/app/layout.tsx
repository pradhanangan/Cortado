import type { Metadata } from "next";
import "./globals.css";
import Header from "@/components/header";
import Footer from "@/components/footer";
import { Box, CssBaseline, ThemeProvider } from "@mui/material";
export const metadata: Metadata = {
  title: "Cortado: Order Ticket",
  description: "",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" translate="no">
      <head>
        <meta name="google" content="notranslate" />
        <meta httpEquiv="Content-Language" content="en" />
        <meta name="language" content="English" />
      </head>
      <body>
        <Box
          component={"main"}
          style={{
            display: "flex",
            flexDirection: "column",
            minHeight: "100vh",
          }}
        >
          <Header />
          <Box sx={{ flex: 1, my: 4 }}>{children}</Box>
          <Footer />
        </Box>
      </body>
    </html>
  );
}
