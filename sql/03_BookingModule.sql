CREATE TABLE orders (
    id uuid NOT NULL,
    order_number text NOT NULL,
    product_id uuid NOT NULL,
    email text NOT NULL,
    phone_number text NOT NULL,
    first_name text NOT NULL,
    last_name text NOT NULL,
    order_date timestamp with time zone NOT NULL,
    sub_total numeric NOT NULL, -- Sum of LineItem.LineTotal (before tax and discounts)
    total_amount numeric NOT NULL, -- Final amount: SubTotal - Discount + Tax
    is_verified boolean NOT NULL,
    is_paid boolean NOT NULL,
    payment_id text NOT NULL,
    is_confirmed boolean NOT NULL,
    created_by text NOT NULL,
    created_date timestamp with time zone NOT NULL,
    last_modified_by text NOT NULL,
    last_modified_date timestamp with time zone,
    CONSTRAINT pk_orders PRIMARY KEY (id),
    CONSTRAINT fk_orders_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE CASCADE
);

CREATE SEQUENCE order_number_seq AS integer START WITH 100000000 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;

CREATE TABLE order_items (
    id uuid NOT NULL,
    order_id uuid NOT NULL,
    product_item_id uuid NOT NULL,
    unit_price numeric NOT NULL,
    quantity integer NOT NULL,
    line_total numeric NOT NULL,
    created_by text NOT NULL,
    created_date timestamp with time zone NOT NULL,
    last_modified_by text NOT NULL,
    last_modified_date timestamp with time zone,
    CONSTRAINT pk_order_items PRIMARY KEY (id),
    CONSTRAINT fk_order_items_orders_order_id FOREIGN KEY (order_id) REFERENCES orders (id) ON DELETE CASCADE,
    CONSTRAINT fk_order_items_product_items_product_item_id FOREIGN KEY (product_item_id) REFERENCES product_items (id) ON DELETE CASCADE
);

CREATE TABLE tickets (
    id uuid NOT NULL,
    order_item_id uuid NOT NULL,
    ticket_number text NOT NULL,
    is_used boolean NOT NULL,
    used_date timestamp with time zone,
    price numeric NOT NULL,
    status text NOT NULL,
    qr_code bytea,
    created_by text NOT NULL,
    created_date timestamp with time zone NOT NULL,
    last_modified_by text NOT NULL,
    last_modified_date timestamp with time zone,
    CONSTRAINT pk_tickets PRIMARY KEY (id),
    CONSTRAINT fk_tickets_order_items_order_item_id FOREIGN KEY (order_item_id) REFERENCES order_items (id) ON DELETE CASCADE
);

CREATE SEQUENCE ticket_number_seq AS integer START WITH 100000000 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;

