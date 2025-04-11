CREATE TABLE customers (
    id uuid NOT NULL,
    username text NOT NULL,
    email text NOT NULL,
    identity_id uuid NOT NULL,
    created_by text NOT NULL,
    created_date timestamp with time zone NOT NULL,
    last_modified_by text NOT NULL,
    last_modified_date timestamp with time zone,
    CONSTRAINT pk_customers PRIMARY KEY (id)
);