"use client";
import Link from "next/link";
import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  Container,
  Typography,
} from "@mui/material";
import { ACTIVITIES } from "@/data/data";
import { useEffect, useState } from "react";
import { Product, ProductItem } from "@/types/products-module";

export default function Home() {
  const [products, setProducts] = useState<Product[]>([]);
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await fetch(`https://localhost:7159/api/products`, {
          method: "GET",
        });

        if (response.ok) {
          const data = await response.json();
          setProducts(data);
        } else {
          throw new Error("Failed to fetch activities");
        }
      } catch (error) {
        console.error(error);
      }
    };
    fetchProducts();
  }, []);

  return (
    <Box>
      <Container>
        <Box display="flex" justifyContent="flex-left" marginBottom={4}>
          <Typography variant="h5" component="h5" align="center">
            Upcoming Events
          </Typography>
        </Box>
        <Box display="flex" flexWrap="wrap" justifyContent="flex-start" gap={2}>
          {products.map((product) => (
            <Card
              key={product.code}
              variant="outlined"
              sx={{
                minWidth: 275,
                height: 200,
                display: "flex",
                flexDirection: "column",
                justifyContent: "space-between",
              }}
            >
              <CardContent>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    flexGrow: 1,
                  }}
                >
                  <Typography variant="h6" component="h2">
                    {product.description}
                  </Typography>
                </Box>
              </CardContent>

              <CardActions>
                <Box display="flex" justifyContent="center" width="100%">
                  <Link href={`/orders?productId=${product.id}`} passHref>
                    <Button variant="contained" color="primary">
                      Book
                    </Button>
                  </Link>
                </Box>
              </CardActions>
            </Card>
          ))}
        </Box>
      </Container>
    </Box>
  );
}
