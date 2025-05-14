CREATE TABLE orders (
    id UUID NOT NULL,
    order_number VARCHAR(20) NOT NULL,
    product_id UUID NOT NULL,
    email VARCHAR(320) NOT NULL,
    phone_number VARCHAR(20) NOT NULL,
    first_name VARCHAR(40) NOT NULL,
    last_name VARCHAR(40) NOT NULL,
    order_date TIMESTAMP WITH TIME ZONE NOT NULL,
    sub_total NUMERIC NOT NULL, -- Sum of LineItem.LineTotal (before tax and discounts)
    total_amount NUMERIC NOT NULL, -- Final amount: SubTotal - Discount + Tax
    is_verified BOOLEAN NOT NULL,
    is_paid BOOLEAN NOT NULL,
    payment_id VARCHAR(20) NOT NULL,
    is_confirmed BOOLEAN NOT NULL,
    created_by VARCHAR(40) NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    last_modified_by VARCHAR(40) NOT NULL,
    last_modified_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT pk_orders PRIMARY KEY (id),
    CONSTRAINT fk_orders_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE CASCADE
);

CREATE SEQUENCE order_number_seq AS INTEGER START WITH 100000000 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;

CREATE TABLE order_items (
    id UUID NOT NULL,
    order_id UUID NOT NULL,
    product_item_id UUID NOT NULL,
    unit_price NUMERIC NOT NULL,
    quantity INTEGER NOT NULL,
    line_total NUMERIC NOT NULL,
    created_by VARCHAR(40) NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    last_modified_by VARCHAR(40) NOT NULL,
    last_modified_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT pk_order_items PRIMARY KEY (id),
    CONSTRAINT fk_order_items_orders_order_id FOREIGN KEY (order_id) REFERENCES orders (id) ON DELETE CASCADE,
    CONSTRAINT fk_order_items_product_items_product_item_id FOREIGN KEY (product_item_id) REFERENCES product_items (id) ON DELETE CASCADE
);

CREATE TABLE tickets (
    id UUID NOT NULL,
    order_item_id UUID NOT NULL,
    ticket_number VARCHAR(20) NOT NULL,
    is_used BOOLEAN NOT NULL,
    used_date TIMESTAMP WITH TIME ZONE,
    price NUMERIC NOT NULL,
    status VARCHAR(20) NOT NULL,
    qr_code BYTEA,
    created_by VARCHAR(40) NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    last_modified_by VARCHAR(40) NOT NULL,
    last_modified_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT pk_tickets PRIMARY KEY (id),
    CONSTRAINT fk_tickets_order_items_order_item_id FOREIGN KEY (order_item_id) REFERENCES order_items (id) ON DELETE CASCADE
);

CREATE SEQUENCE ticket_number_seq AS INTEGER START WITH 100000000 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;

