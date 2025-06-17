import { Box, Divider, Typography } from "@mui/material";

export default function Footer() {
  const currentYear = new Date().getFullYear();
  return (
    <>
      <Divider />
      <Box
        component={"footer"}
        padding={3}
        sx={{
          mt: "auto",
          height: "80px",
          display: "flex",
          alignItems: "center",
        }}
      >
        <Typography variant="body2">
          &copy; {currentYear} NPradhananga
        </Typography>
      </Box>
    </>
  );
}
