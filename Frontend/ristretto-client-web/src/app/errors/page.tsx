import { Container, Alert } from "@mui/material";
export default function ErrorPage() {
  return (
    <Container>
      <Alert severity="error">Something went wrong!!!</Alert>
    </Container>
  );
}
