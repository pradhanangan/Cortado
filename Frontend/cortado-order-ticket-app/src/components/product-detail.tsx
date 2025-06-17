import Image from "next/image";
import { Box, Container, Typography } from "@mui/material";
import Grid from "@mui/material/Grid2";
import CalendarMonthIcon from "@mui/icons-material/CalendarMonth";
import ScheduleIcon from "@mui/icons-material/Schedule";
import LocationOnIcon from "@mui/icons-material/LocationOn";

import { Product } from "@/types/product-type";
import { formatDate, formatTime } from "@/utils/date-utils";
interface ProductDetailProps {
  product: Product;
}
export default function ProductDetail({ product }: ProductDetailProps) {
  return (
    <Box component="main" sx={{ flexGrow: 1 }}>
      <Box>
        <Container>
          <Grid container spacing={3} alignItems="center">
            <Grid size={{ xs: 12, lg: 6 }} order={{ xs: 1, lg: 1 }}>
              <Typography variant="h4" gutterBottom sx={{ mb: 1 }}>
                {product.name}
              </Typography>
              <Typography
                variant="body1"
                sx={{
                  fontSize: { xs: "1rem", md: "1.125rem" },
                  mb: 1.5,
                  color: "text.secondary",
                }}
              >
                {product.description}
              </Typography>
              <Box
                sx={{
                  display: "flex",
                  flexDirection: "column",
                  gap: 1,
                  color: "text.secondary",
                  // fontSize: "0.875rem",
                }}
              >
                <Typography
                  variant="body2"
                  sx={{ display: "flex", alignItems: "center", gap: 1 }}
                >
                  <CalendarMonthIcon fontSize="small" />
                  {formatDate(product.startDate, "en-GB")}
                </Typography>
                <Typography
                  variant="body2"
                  sx={{ display: "flex", alignItems: "center", gap: 1 }}
                >
                  <ScheduleIcon fontSize="small" />
                  Gates open at {formatTime(product.startTime)}, Event ends at{" "}
                  {formatTime(product.endTime)}
                </Typography>
                <Typography
                  variant="body2"
                  sx={{ display: "flex", alignItems: "center", gap: 1 }}
                >
                  <LocationOnIcon fontSize="small" />
                  {product.address}
                </Typography>
              </Box>
            </Grid>
            <Grid size={{ xs: 12, lg: 6 }} order={{ xs: 2, lg: 2 }}>
              <Box
                sx={{
                  borderRadius: 2,
                  overflow: "hidden",
                  boxShadow: 1,
                  aspectRatio: "16/9",
                  position: "relative",
                  width: "100%",
                  height: { xs: "180px", sm: "220px", md: "250px" },
                }}
              >
                <Image
                  src={product.imageUrl || "https://picsum.photos/400/200"}
                  alt="Summer Music Festival"
                  fill
                  style={{ objectFit: "cover" }}
                  sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
                  priority
                />
              </Box>
            </Grid>
          </Grid>
        </Container>
      </Box>
    </Box>
  );
}
