PGDMP     7                
    {            Food_Delivery    15.4    15.4 4    S           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            T           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            U           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            V           1262    16414    Food_Delivery    DATABASE     �   CREATE DATABASE "Food_Delivery" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
    DROP DATABASE "Food_Delivery";
                postgres    false            �            1255    16624    calculate_total_cost()    FUNCTION     �  CREATE FUNCTION public.calculate_total_cost() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
update orders SET totalcost = totalcost - (select sum(d.dish_cost * OLD.quantity) from dish as d where d.id_dish = OLD.id_dish_fk)
where id_orders = OLD.id_orders_fk;
update orders SET totalcost = totalcost + (select sum(d.dish_cost * NEW.quantity) from dish as d where d.id_dish = NEW.id_dish_fk)
where id_orders = NEW.id_orders_fk;
RETURN NEW;
END;
$$;
 -   DROP FUNCTION public.calculate_total_cost();
       public          postgres    false            �            1255    16625    calculate_total_cost_delete()    FUNCTION     -  CREATE FUNCTION public.calculate_total_cost_delete() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
update orders SET totalcost = totalcost - (select sum(d.dish_cost * o.quantity) from OLD as o
join dish as d on d.id_dish = o.id_dish_fk)
where id_orders = OLD.id_orders_fk;
RETURN NEW;
END;
$$;
 4   DROP FUNCTION public.calculate_total_cost_delete();
       public          postgres    false            �            1255    16626    calculate_total_cost_insert()    FUNCTION     -  CREATE FUNCTION public.calculate_total_cost_insert() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
update orders SET totalcost = totalcost + (select sum(d.dish_cost * o.quantity) from NEW as o
join dish as d on d.id_dish = o.id_dish_fk)
where id_orders = NEW.id_orders_fk;
RETURN NEW;
END;
$$;
 4   DROP FUNCTION public.calculate_total_cost_insert();
       public          postgres    false            �            1255    16627    calculate_total_cost_update()    FUNCTION     �  CREATE FUNCTION public.calculate_total_cost_update() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
update orders SET totalcost = totalcost - (select sum(d.dish_cost * o.quantity) from OLD as o
join dish as d on d.id_dish = o.id_dish_fk)
where id_orders = OLD.id_orders_fk;
update orders SET totalcost = totalcost + (select sum(d.dish_cost * o.quantity) from NEW as o
join dish as d on d.id_dish = o.id_dish_fk)
where id_orders = NEW.id_orders_fk;
RETURN NEW;
END;
$$;
 4   DROP FUNCTION public.calculate_total_cost_update();
       public          postgres    false            �            1259    16436    customer    TABLE     �  CREATE TABLE public.customer (
    id_customer integer NOT NULL,
    customer_lastname character varying(80) DEFAULT 'Иванов'::character varying NOT NULL,
    customer_firstname character varying(80) DEFAULT 'Иван'::character varying NOT NULL,
    customer_patronymic character varying(80),
    customer_phonenumber character(12) NOT NULL,
    city character varying(50) NOT NULL,
    street character varying(50) NOT NULL,
    house_number smallint NOT NULL,
    building character(1),
    apartment smallint,
    foto character varying(50),
    CONSTRAINT check_apartment_bigger_zero CHECK ((apartment > 0)),
    CONSTRAINT client_house_number_check CHECK ((house_number > 0))
);
    DROP TABLE public.customer;
       public         heap    postgres    false            �            1259    16435    client_id_client_seq    SEQUENCE     �   ALTER TABLE public.customer ALTER COLUMN id_customer ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.client_id_client_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    219            �            1259    16424    curier    TABLE     �  CREATE TABLE public.curier (
    id_curier integer NOT NULL,
    curier_lastname character varying(80) DEFAULT 'Петров'::character varying NOT NULL,
    curier_firstname character varying(80) DEFAULT 'Петр'::character varying NOT NULL,
    curier_patronymic character varying(80),
    curier_phonenumber character(11) NOT NULL,
    delivery_type character varying(10) NOT NULL,
    birthday date NOT NULL,
    passport_series character(4) NOT NULL,
    passport_number character(6) NOT NULL,
    passport_issuedby character varying(255) NOT NULL,
    passport_department character(7) NOT NULL,
    CONSTRAINT curier_birthday_check CHECK (((CURRENT_DATE - birthday) > 18)),
    CONSTRAINT curier_delivery_type_check CHECK (((delivery_type)::text = ANY ((ARRAY['Пешая'::character varying, 'Велосипед'::character varying, 'Автомобиль'::character varying])::text[])))
);
    DROP TABLE public.curier;
       public         heap    postgres    false            �            1259    16423    curier_id_curier_seq    SEQUENCE     �   ALTER TABLE public.curier ALTER COLUMN id_curier ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.curier_id_curier_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    217            �            1259    16486    deliverylist    TABLE     �  CREATE TABLE public.deliverylist (
    id_deliverylist integer NOT NULL,
    id_orders_fk integer NOT NULL,
    id_curier_fk integer NOT NULL,
    time_delivered timestamp without time zone,
    payment_type character varying(11) NOT NULL,
    delivery_completion character varying(6),
    CONSTRAINT delivery_completion_values CHECK (((delivery_completion)::text = ANY ((ARRAY['Да'::character varying, 'Нет'::character varying, 'Отмена'::character varying])::text[]))),
    CONSTRAINT deliverylist_payment_type_check CHECK (((payment_type)::text = ANY ((ARRAY['Наличная'::character varying, 'Безналичная'::character varying])::text[])))
);
     DROP TABLE public.deliverylist;
       public         heap    postgres    false            �            1259    16485     deliverylist_id_deliverylist_seq    SEQUENCE     �   ALTER TABLE public.deliverylist ALTER COLUMN id_deliverylist ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.deliverylist_id_deliverylist_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    225            �            1259    16416    dish    TABLE     �   CREATE TABLE public.dish (
    id_dish integer NOT NULL,
    dish_name character varying(80) NOT NULL,
    dish_cost numeric(10,2) NOT NULL
);
    DROP TABLE public.dish;
       public         heap    postgres    false            �            1259    16415    dish_id_dish_seq    SEQUENCE     �   ALTER TABLE public.dish ALTER COLUMN id_dish ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dish_id_dish_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    215            �            1259    16469    dish_order_list    TABLE     �   CREATE TABLE public.dish_order_list (
    id_dish_order_list integer NOT NULL,
    id_orders_fk integer NOT NULL,
    id_dish_fk integer NOT NULL,
    quantity integer NOT NULL,
    CONSTRAINT dish_order_list_quantity_check CHECK ((quantity > 0))
);
 #   DROP TABLE public.dish_order_list;
       public         heap    postgres    false            �            1259    16468 &   dish_order_list_id_dish_order_list_seq    SEQUENCE     �   ALTER TABLE public.dish_order_list ALTER COLUMN id_dish_order_list ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dish_order_list_id_dish_order_list_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    223            �            1259    16620    dish_order_list_view    VIEW     �   CREATE VIEW public.dish_order_list_view AS
 SELECT d.id_dish_order_list,
    d.id_orders_fk,
    ds.dish_name,
    d.quantity
   FROM (public.dish_order_list d
     JOIN public.dish ds ON ((ds.id_dish = d.id_dish_fk)));
 '   DROP VIEW public.dish_order_list_view;
       public          postgres    false    223    223    223    215    215    223            �            1259    16449    orders    TABLE       CREATE TABLE public.orders (
    id_orders integer NOT NULL,
    id_customer_fk integer NOT NULL,
    time_ordered timestamp without time zone DEFAULT now() NOT NULL,
    totalcost numeric(10,2) NOT NULL,
    CONSTRAINT orders_totalcost_check CHECK ((totalcost > (0)::numeric))
);
    DROP TABLE public.orders;
       public         heap    postgres    false            �            1259    16462    orders_id_orders_seq    SEQUENCE     �   ALTER TABLE public.orders ALTER COLUMN id_orders ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.orders_id_orders_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    220            �            1259    16616    orders_view    VIEW     !  CREATE VIEW public.orders_view AS
 SELECT o.id_orders,
    co.id_customer,
    co.customer_lastname,
    co.customer_firstname,
    co.customer_patronymic,
    o.time_ordered,
    o.totalcost
   FROM (public.orders o
     JOIN public.customer co ON ((co.id_customer = o.id_customer_fk)));
    DROP VIEW public.orders_view;
       public          postgres    false    220    219    219    219    219    220    220    220            �            1259    16504 	   userlogin    TABLE     �   CREATE TABLE public.userlogin (
    id integer NOT NULL,
    username character varying(64) NOT NULL,
    passcode character varying(64) NOT NULL,
    status integer DEFAULT 1,
    additionalid integer
);
    DROP TABLE public.userlogin;
       public         heap    postgres    false            �            1259    16503    userlogin_id_seq    SEQUENCE     �   ALTER TABLE public.userlogin ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.userlogin_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    227            F          0    16424    curier 
   TABLE DATA           �   COPY public.curier (id_curier, curier_lastname, curier_firstname, curier_patronymic, curier_phonenumber, delivery_type, birthday, passport_series, passport_number, passport_issuedby, passport_department) FROM stdin;
    public          postgres    false    217   �L       H          0    16436    customer 
   TABLE DATA           �   COPY public.customer (id_customer, customer_lastname, customer_firstname, customer_patronymic, customer_phonenumber, city, street, house_number, building, apartment, foto) FROM stdin;
    public          postgres    false    219   NN       N          0    16486    deliverylist 
   TABLE DATA           �   COPY public.deliverylist (id_deliverylist, id_orders_fk, id_curier_fk, time_delivered, payment_type, delivery_completion) FROM stdin;
    public          postgres    false    225   gP       D          0    16416    dish 
   TABLE DATA           =   COPY public.dish (id_dish, dish_name, dish_cost) FROM stdin;
    public          postgres    false    215   Q       L          0    16469    dish_order_list 
   TABLE DATA           a   COPY public.dish_order_list (id_dish_order_list, id_orders_fk, id_dish_fk, quantity) FROM stdin;
    public          postgres    false    223   �Q       I          0    16449    orders 
   TABLE DATA           T   COPY public.orders (id_orders, id_customer_fk, time_ordered, totalcost) FROM stdin;
    public          postgres    false    220   oR       P          0    16504 	   userlogin 
   TABLE DATA           Q   COPY public.userlogin (id, username, passcode, status, additionalid) FROM stdin;
    public          postgres    false    227   �R       W           0    0    client_id_client_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.client_id_client_seq', 22, true);
          public          postgres    false    218            X           0    0    curier_id_curier_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.curier_id_curier_seq', 17, true);
          public          postgres    false    216            Y           0    0     deliverylist_id_deliverylist_seq    SEQUENCE SET     O   SELECT pg_catalog.setval('public.deliverylist_id_deliverylist_seq', 10, true);
          public          postgres    false    224            Z           0    0    dish_id_dish_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.dish_id_dish_seq', 13, true);
          public          postgres    false    214            [           0    0 &   dish_order_list_id_dish_order_list_seq    SEQUENCE SET     U   SELECT pg_catalog.setval('public.dish_order_list_id_dish_order_list_seq', 52, true);
          public          postgres    false    222            \           0    0    orders_id_orders_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.orders_id_orders_seq', 27, true);
          public          postgres    false    221            ]           0    0    userlogin_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.userlogin_id_seq', 19, true);
          public          postgres    false    226            �           2606    16443    customer client_pkey 
   CONSTRAINT     [   ALTER TABLE ONLY public.customer
    ADD CONSTRAINT client_pkey PRIMARY KEY (id_customer);
 >   ALTER TABLE ONLY public.customer DROP CONSTRAINT client_pkey;
       public            postgres    false    219            �           2606    16434    curier curier_pkey 
   CONSTRAINT     W   ALTER TABLE ONLY public.curier
    ADD CONSTRAINT curier_pkey PRIMARY KEY (id_curier);
 <   ALTER TABLE ONLY public.curier DROP CONSTRAINT curier_pkey;
       public            postgres    false    217            �           2606    16492    deliverylist deliverylist_pkey 
   CONSTRAINT     i   ALTER TABLE ONLY public.deliverylist
    ADD CONSTRAINT deliverylist_pkey PRIMARY KEY (id_deliverylist);
 H   ALTER TABLE ONLY public.deliverylist DROP CONSTRAINT deliverylist_pkey;
       public            postgres    false    225            �           2606    16422    dish dish_dish_name_key 
   CONSTRAINT     W   ALTER TABLE ONLY public.dish
    ADD CONSTRAINT dish_dish_name_key UNIQUE (dish_name);
 A   ALTER TABLE ONLY public.dish DROP CONSTRAINT dish_dish_name_key;
       public            postgres    false    215            �           2606    16474 $   dish_order_list dish_order_list_pkey 
   CONSTRAINT     r   ALTER TABLE ONLY public.dish_order_list
    ADD CONSTRAINT dish_order_list_pkey PRIMARY KEY (id_dish_order_list);
 N   ALTER TABLE ONLY public.dish_order_list DROP CONSTRAINT dish_order_list_pkey;
       public            postgres    false    223            �           2606    16420    dish dish_pkey 
   CONSTRAINT     Q   ALTER TABLE ONLY public.dish
    ADD CONSTRAINT dish_pkey PRIMARY KEY (id_dish);
 8   ALTER TABLE ONLY public.dish DROP CONSTRAINT dish_pkey;
       public            postgres    false    215            �           2606    16461    orders orders_pkey 
   CONSTRAINT     W   ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (id_orders);
 <   ALTER TABLE ONLY public.orders DROP CONSTRAINT orders_pkey;
       public            postgres    false    220            �           2606    16509    userlogin userlogin_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.userlogin
    ADD CONSTRAINT userlogin_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.userlogin DROP CONSTRAINT userlogin_pkey;
       public            postgres    false    227            �           2620    16628 )   dish_order_list total_cost_trigger_insert    TRIGGER     �   CREATE TRIGGER total_cost_trigger_insert AFTER INSERT OR DELETE OR UPDATE ON public.dish_order_list FOR EACH ROW EXECUTE FUNCTION public.calculate_total_cost();
 B   DROP TRIGGER total_cost_trigger_insert ON public.dish_order_list;
       public          postgres    false    223    230            �           2606    16498 +   deliverylist deliverylist_id_curier_fk_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.deliverylist
    ADD CONSTRAINT deliverylist_id_curier_fk_fkey FOREIGN KEY (id_curier_fk) REFERENCES public.curier(id_curier) ON DELETE CASCADE;
 U   ALTER TABLE ONLY public.deliverylist DROP CONSTRAINT deliverylist_id_curier_fk_fkey;
       public          postgres    false    217    3234    225            �           2606    16493 +   deliverylist deliverylist_id_orders_fk_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.deliverylist
    ADD CONSTRAINT deliverylist_id_orders_fk_fkey FOREIGN KEY (id_orders_fk) REFERENCES public.orders(id_orders) ON DELETE CASCADE;
 U   ALTER TABLE ONLY public.deliverylist DROP CONSTRAINT deliverylist_id_orders_fk_fkey;
       public          postgres    false    220    3238    225            �           2606    16480 /   dish_order_list dish_order_list_id_dish_fk_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.dish_order_list
    ADD CONSTRAINT dish_order_list_id_dish_fk_fkey FOREIGN KEY (id_dish_fk) REFERENCES public.dish(id_dish) ON DELETE CASCADE;
 Y   ALTER TABLE ONLY public.dish_order_list DROP CONSTRAINT dish_order_list_id_dish_fk_fkey;
       public          postgres    false    215    3232    223            �           2606    16475 1   dish_order_list dish_order_list_id_orders_fk_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.dish_order_list
    ADD CONSTRAINT dish_order_list_id_orders_fk_fkey FOREIGN KEY (id_orders_fk) REFERENCES public.orders(id_orders) ON DELETE CASCADE;
 [   ALTER TABLE ONLY public.dish_order_list DROP CONSTRAINT dish_order_list_id_orders_fk_fkey;
       public          postgres    false    223    220    3238            �           2606    16463    orders fk_id_customer    FK CONSTRAINT     �   ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_id_customer FOREIGN KEY (id_customer_fk) REFERENCES public.customer(id_customer) ON DELETE CASCADE;
 ?   ALTER TABLE ONLY public.orders DROP CONSTRAINT fk_id_customer;
       public          postgres    false    220    3236    219            F   �  x��R[N�0�vN���#���p��G%*U�*8@�6"4M{����U���c[�;;3�JF�h�}�l��K\ִ��7�"?В6~�{C��0딐Jee�Q�oh�o�Bp��Xa��&�Q�^}��z��.G��_��w�����c?y�Ӂ�9�G9=Fp�ZDy�ٲ�n���q�D�'�<Q類D��m �C�(�p����(+2Bi0��(����[!��M�vX�@�gZ��n��y������W�d ���,d���q����6��f�vJ�/ʀj���Q̤s���&H�V��Q�I�~���[���vRp�UV0໘H���)��"��cӞ��4�*#�FF��?�XV)SV�O�OS2��}ҏy���2Uv1ʲ��o�~      H   	  x��S�j�P}�|Eߋ%��������'/ ��x*ZPh��R+��fz	���0��\3��QI���dggf�Y3k⯩�nxA��gX\�8=��%�yq˳��K��-
ol�f��'r��`�!��K,eI�&�i�>����.P�T<p6�ky�F<���#�V8D�9�L��ޗ��|�~ �����B)���$�Ju:�/)��|��1v���JM=��e4x�a�3ݘl!��^`MnE�ޙ���N՚aB|�:���"��/}���B����B��5=Mt��Hǈϕ� �LeM�0���IM$=QE20�7C�od��]��V�Qz��{1ϋ�p���� Bf7G��9��~.@�GF�[�S-�����Or��s���t����O���Wb�5��j��?=0�1���=y�t�����@�GL�P�=�ԽWC�������k�.xX݅�n���X���N�t�Q�P+���{����	��<�N��3�
�Ag�t�v��F��ɫ�k�A:���ɲ�h$�.      N   �   x�����0E��,P;IS:t�^8"$�e�^*��;�l��1�|����'3��X���{`�;�[K�a�+f��J��KW��r�\����T�QS)������?((��Cͪ�V��sa�%�&RT�b��F|t����n���������?      D   �   x�M�=j�@���)�F�J"D�b�.��dH��\��l����+�����2�-f�o�1���38��Zk8���A_���V�+֥�n�@����t�?}E�o8B'8��ﱡ`Þ�%A�F_��Dݵ�t����M�$O�46����8|�-�����ʞ�Zn��z�_h�E�O����@9�d��X���;3����L��4�<�=d�{���:��      L   `   x�%�� !C��0�����Z._%�1,t0xs�dZb�,m�M.��KZz_r���;tϕ�(�G�����������u��W�Uvx �c,      I   [   x��ϻ�0 �����ـg��sD=I�t�10��L7�%�ɷ� ^�x�"���S��e�Q�+�b�eZl��9�EŢW��D|�~8/      P   �   x���;n�0��<L!>DIsе[� �-�A�6�����#��@�	.ۺ>�
A�jI�l�F�&��h��c����Xj%�Q5�F2���T�ڬ� ��y_��[����x���_����ؒ@1������:�NX�8a�cꄕ�E'�B��y-��=��C҃��Y�xPEP���6mS�1g� ,x�@���     