import { AppBar, Box, Toolbar, Typography } from "@mui/material";

export default function Header() {
  return (
    <Box>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Ristretto
          </Typography>
        </Toolbar>
      </AppBar>
    </Box>
  );
}
