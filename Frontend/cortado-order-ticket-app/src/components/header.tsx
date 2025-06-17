import { AppBar, Box, Toolbar, Typography } from "@mui/material";
import LocalCafeIcon from "@mui/icons-material/LocalCafe";

export default function Header() {
  return (
    <Box>
      <AppBar position="static">
        <Toolbar>
          <LocalCafeIcon />
          <Typography
            variant="h6"
            component="div"
            sx={{ flexGrow: 1, marginLeft: 1 }}
          >
            Cortado
          </Typography>
          <Typography variant="caption">Order Ticket</Typography>
        </Toolbar>
      </AppBar>
    </Box>
  );
}
