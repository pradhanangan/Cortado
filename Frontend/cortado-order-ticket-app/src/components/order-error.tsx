import { Container, Stack, Typography } from "@mui/material";
interface OrderErrorProps {
  message?: string;
}

export default function OrderError({
  message = "Something went wrong. Please try again.",
}: OrderErrorProps) {
  return (
    <Container
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        pt: { xs: 14, sm: 20 },
        pb: { xs: 8, sm: 12 },
      }}
    >
      <Stack
        spacing={2}
        useFlexGap
        sx={{ alignItems: "center", width: { xs: "100%", sm: "70%" } }}
      >
        <Typography
          variant="h1"
          sx={{
            display: "flex",
            flexDirection: { xs: "column", sm: "row" },
            alignItems: "center",
            fontSize: "clamp(3rem, 10vw, 3.5rem)",
          }}
        >
          {/* <Typography
                  component="span"
                  variant="h1"
                  sx={(theme) => ({
                    fontSize: "inherit",
                    color: "primary.main",
                    ...theme.applyStyles("dark", {
                      color: "primary.light",
                    }),
                  })}
                >
                  Cortado
                </Typography> */}
          Oops!
        </Typography>
        <Typography
          sx={{
            textAlign: "center",
            color: "text.secondary",
            width: { sm: "100%", md: "80%" },
          }}
        >
          {message}
        </Typography>
      </Stack>
    </Container>
  );
}
