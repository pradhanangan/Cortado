import { alpha, createTheme } from "@mui/material";

const theme = createTheme({
  typography: {
    fontFamily: "Roboto, Arial, sans-serif",
  },
  palette: {
    primary: {
      main: "#1976d2",
    },
    secondary: {
      main: "#dc004e",
    },
    text: {
      primary: alpha("#000000DE", 0.87),
      secondary: alpha("#000000DE", 0.6),
    },
  },
});

export default theme;
